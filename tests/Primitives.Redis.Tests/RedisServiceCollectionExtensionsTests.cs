using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Forge.Primitives.Redis.Tests;

public sealed class RedisServiceCollectionExtensionsTests
{
    // 192.0.2.0/24 (TEST-NET-1, RFC 5737) is reserved for documentation and never routable, so it
    // fails fast without a real DNS/connect timeout - proving the DI registration shape, not real
    // connectivity. AbortOnConnectFail stays false (the primitive's default) so Connect() returns a
    // disconnected multiplexer instead of throwing.
    private static RedisOptions UnreachableOptions(int database = 0) => new()
    {
        Host = "192.0.2.1",
        ConnectTimeout = 100,
        ConnectRetry = 0,
        DefaultDatabase = database,
    };

    [Fact]
    public void AddKeyedConnectionMultiplexer_RegistersResolvableKeyedService()
    {
        var services = new ServiceCollection();
        services.AddKeyedConnectionMultiplexer("app", UnreachableOptions());
        using var provider = services.BuildServiceProvider();

        var multiplexer = provider.GetRequiredKeyedService<IConnectionMultiplexer>("app");

        Assert.NotNull(multiplexer);
    }

    [Fact]
    public void AddKeyedConnectionMultiplexer_DifferentKeysResolveIndependently()
    {
        var services = new ServiceCollection();
        services.AddKeyedConnectionMultiplexer("a", UnreachableOptions(database: 1));
        services.AddKeyedConnectionMultiplexer("b", UnreachableOptions(database: 2));
        using var provider = services.BuildServiceProvider();

        var a = provider.GetRequiredKeyedService<IConnectionMultiplexer>("a");
        var b = provider.GetRequiredKeyedService<IConnectionMultiplexer>("b");

        Assert.NotSame(a, b);
    }
}
