using Primitives.MongoDb;

namespace Primitives.MongoDb.Tests;

public sealed class OnceGateTests
{
    [Fact]
    public void EnsureRun_RunsTheActionOnlyOnce()
    {
        var gate = new OnceGate();
        var runs = 0;

        gate.EnsureRun(() => runs++);
        gate.EnsureRun(() => runs++);
        gate.EnsureRun(() => runs++);

        Assert.Equal(1, runs);
    }

    [Fact]
    public async Task EnsureRun_UnderConcurrentCallers_StillRunsExactlyOnceAndBlocksUntilComplete()
    {
        var gate = new OnceGate();
        var runs = 0;
        var started = new ManualResetEventSlim();
        var proceed = new ManualResetEventSlim();

        // The first caller's action blocks until released, proving a concurrent second caller
        // is blocked waiting for it rather than seeing "done" early (the TOCTOU race a plain
        // Interlocked.Exchange-guarded bool would allow).
        var first = Task.Run(() => gate.EnsureRun(() =>
        {
            Interlocked.Increment(ref runs);
            started.Set();
            proceed.Wait();
        }));

        Assert.True(started.Wait(TimeSpan.FromSeconds(5)));
        var second = Task.Run(() => gate.EnsureRun(() => Interlocked.Increment(ref runs)));

        // The second caller must still be waiting on the lock, not having run its own action.
        var secondCompletedEarly = await Task.WhenAny(second, Task.Delay(TimeSpan.FromMilliseconds(100))) == second;
        Assert.False(secondCompletedEarly);

        proceed.Set();
        var all = Task.WhenAll(first, second);
        Assert.True(await Task.WhenAny(all, Task.Delay(TimeSpan.FromSeconds(5))) == all);
        Assert.Equal(1, runs);
    }

    [Fact]
    public void EnsureRun_RejectsANullAction()
    {
        Assert.Throws<ArgumentNullException>(() => new OnceGate().EnsureRun(null!));
    }
}
