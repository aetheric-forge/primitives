namespace Forge.Primitives.Redis.Tests;

public sealed class RedisOptionsTests
{
    [Fact]
    public void ToConfigurationOptions_MapsCoreFields()
    {
        var options = new RedisOptions
        {
            Host = "redis.internal",
            Port = 6380,
            User = "app",
            Password = "s3cret",
            Ssl = true,
            DefaultDatabase = 3,
            AbortOnConnectFail = true,
        };

        var configurationOptions = options.ToConfigurationOptions();

        Assert.Single(configurationOptions.EndPoints);
        Assert.Contains("redis.internal:6380", configurationOptions.EndPoints[0].ToString());
        Assert.Equal("app", configurationOptions.User);
        Assert.Equal("s3cret", configurationOptions.Password);
        Assert.True(configurationOptions.Ssl);
        Assert.Equal(3, configurationOptions.DefaultDatabase);
        Assert.True(configurationOptions.AbortOnConnectFail);
    }

    [Fact]
    public void ToConfigurationOptions_ConnectRetryAndTimeout_UnsetByDefault()
    {
        var options = new RedisOptions { Host = "redis.internal" };

        var configurationOptions = options.ToConfigurationOptions();

        Assert.Equal(3, configurationOptions.ConnectRetry);
        Assert.Equal(5000, configurationOptions.ConnectTimeout);
    }

    [Fact]
    public void ToConfigurationOptions_ConnectRetryAndTimeout_AppliedWhenProvided()
    {
        var options = new RedisOptions
        {
            Host = "redis.internal",
            ConnectRetry = 1,
            ConnectTimeout = 5000,
        };

        var configurationOptions = options.ToConfigurationOptions();

        Assert.Equal(1, configurationOptions.ConnectRetry);
        Assert.Equal(5000, configurationOptions.ConnectTimeout);
    }

    [Fact]
    public void ToConfigurationOptions_DefaultsMatchLocalDevExpectations()
    {
        var options = new RedisOptions { Host = "localhost" };

        var configurationOptions = options.ToConfigurationOptions();

        Assert.Equal(0, configurationOptions.DefaultDatabase);
        Assert.False(configurationOptions.Ssl);
        Assert.False(configurationOptions.AbortOnConnectFail);
        Assert.Null(configurationOptions.User);
        Assert.Null(configurationOptions.Password);
    }
}
