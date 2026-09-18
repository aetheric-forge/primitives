using StackExchange.Redis;

namespace Forge.Primitives.Redis;

public sealed class RedisOptions
{
    public required string Host { get; set; }
    public int Port { get; set; } = 6379;
    public string? User { get; set; }
    public string? Password { get; set; }
    public bool Ssl { get; set; }
    public int DefaultDatabase { get; set; }
    public bool AbortOnConnectFail { get; set; }
    public int? ConnectRetry { get; set; }
    public int? ConnectTimeout { get; set; }

    public ConfigurationOptions ToConfigurationOptions()
    {
        var options = new ConfigurationOptions
        {
            EndPoints = { { Host, Port } },
            User = User,
            Password = Password,
            Ssl = Ssl,
            DefaultDatabase = DefaultDatabase,
            AbortOnConnectFail = AbortOnConnectFail,
        };

        if (ConnectRetry is { } connectRetry)
        {
            options.ConnectRetry = connectRetry;
        }

        if (ConnectTimeout is { } connectTimeout)
        {
            options.ConnectTimeout = connectTimeout;
        }

        return options;
    }
}
