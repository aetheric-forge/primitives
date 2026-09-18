using Forge.Primitives.MongoDb;

namespace Forge.Primitives.MongoDb.Tests;

public sealed class MongoOptionsTests
{
    [Fact]
    public void ToConnectionString_WithCredentials_IncludesThem()
    {
        var options = new MongoOptions
        {
            Host = "mongo.internal",
            Port = 27018,
            Username = "app",
            Password = "s3cret",
            DatabaseName = "app-db",
        };

        var connectionString = options.ToConnectionString();

        Assert.Contains("app:s3cret@", connectionString);
        Assert.Contains("mongo.internal:27018", connectionString);
        Assert.Contains("authSource=app-db", connectionString);
    }

    [Fact]
    public void ToConnectionString_WithoutUsername_OmitsCredentials()
    {
        var options = new MongoOptions
        {
            Host = "localhost",
            DatabaseName = "local-db",
        };

        var connectionString = options.ToConnectionString();

        Assert.DoesNotContain("@", connectionString);
        Assert.DoesNotContain("authSource", connectionString);
    }

    [Fact]
    public void ToConnectionString_UsesExplicitAuthenticationDatabaseWhenGiven()
    {
        var options = new MongoOptions
        {
            Host = "mongo.internal",
            Username = "app",
            Password = "s3cret",
            DatabaseName = "app-db",
            AuthenticationDatabase = "admin",
        };

        Assert.Contains("authSource=admin", options.ToConnectionString());
    }
}
