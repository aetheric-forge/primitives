using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Forge.Primitives.MongoDb;

namespace Forge.Primitives.MongoDb.Tests;

public sealed class MongoServiceCollectionExtensionsTests
{
    [Fact]
    public void AddKeyedMongoClient_RegistersBothServicesResolvableByKey()
    {
        // MongoClient's constructor never connects eagerly, so an unreachable host is safe here -
        // this test is only proving the DI registration shape, not real connectivity.
        var options = new MongoOptions
        {
            Host = "does-not-resolve.invalid",
            DatabaseName = "app-db",
            Username = "app",
            Password = "s3cret",
        };

        var services = new ServiceCollection();
        services.AddKeyedMongoClient("app", options);
        using var provider = services.BuildServiceProvider();

        var client = provider.GetRequiredKeyedService<IMongoClient>("app");
        var database = provider.GetRequiredKeyedService<IMongoDatabase>("app");

        Assert.NotNull(client);
        Assert.Equal("app-db", database.DatabaseNamespace.DatabaseName);
        Assert.Same(client, database.Client);
    }

    [Fact]
    public void AddKeyedMongoClient_DifferentKeysResolveIndependently()
    {
        var services = new ServiceCollection();
        services.AddKeyedMongoClient("a", new MongoOptions { Host = "a.invalid", DatabaseName = "db-a" });
        services.AddKeyedMongoClient("b", new MongoOptions { Host = "b.invalid", DatabaseName = "db-b" });
        using var provider = services.BuildServiceProvider();

        var databaseA = provider.GetRequiredKeyedService<IMongoDatabase>("a");
        var databaseB = provider.GetRequiredKeyedService<IMongoDatabase>("b");

        Assert.Equal("db-a", databaseA.DatabaseNamespace.DatabaseName);
        Assert.Equal("db-b", databaseB.DatabaseNamespace.DatabaseName);
    }
}
