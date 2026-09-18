using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Forge.Primitives.Redis;

public static class RedisServiceCollectionExtensions
{
    public static IServiceCollection AddKeyedConnectionMultiplexer(this IServiceCollection services, string key, RedisOptions options)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentNullException.ThrowIfNull(options);

        var configurationOptions = options.ToConfigurationOptions();
        services.AddKeyedSingleton<IConnectionMultiplexer>(key, (_, _) => ConnectionMultiplexer.Connect(configurationOptions));
        return services;
    }
}
