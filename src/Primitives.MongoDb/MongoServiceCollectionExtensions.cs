using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Forge.Primitives.MongoDb;

public static class MongoServiceCollectionExtensions
{
    /// <summary>Registers both <see cref="IMongoClient"/> and <see cref="IMongoDatabase"/> under the same key.</summary>
    public static IServiceCollection AddKeyedMongoClient(this IServiceCollection services, string key, MongoOptions options)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentNullException.ThrowIfNull(options);

        var url = MongoUrl.Create(options.ToConnectionString());
        services.AddKeyedSingleton<IMongoClient>(key, (_, _) => new MongoClient(url));
        services.AddKeyedSingleton<IMongoDatabase>(
            key,
            (sp, _) => sp.GetRequiredKeyedService<IMongoClient>(key).GetDatabase(url.DatabaseName));
        return services;
    }
}
