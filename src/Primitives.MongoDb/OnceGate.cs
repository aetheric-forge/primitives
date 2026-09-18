namespace Primitives.MongoDb;

/// <summary>
/// Runs an action exactly once, safely, blocking concurrent callers until the first caller's
/// action has actually finished - not merely started.
///
/// A plain <c>Interlocked.Exchange</c>-guarded bool is a TOCTOU race here: it marks "done"
/// before the action itself runs, so a second caller can see "done" and proceed while the first
/// caller's work - e.g. a MongoDB serializer/convention registration - is still in flight,
/// hitting state that isn't actually ready yet. Double-checked locking closes that window.
/// </summary>
public sealed class OnceGate
{
    private readonly object gate = new();
    private bool ran;

    public void EnsureRun(Action action)
    {
        ArgumentNullException.ThrowIfNull(action);

        if (ran)
        {
            return;
        }

        lock (gate)
        {
            if (ran)
            {
                return;
            }

            action();
            ran = true;
        }
    }
}
