using MongoDB.Driver;

namespace Primitives.MongoDb;

/// <summary>
/// Connection settings for a single MongoDB database. Binds automatically from configuration via
/// the standard <c>IConfiguration.Get&lt;MongoOptions&gt;()</c>/<c>.Configure&lt;MongoOptions&gt;()</c>
/// pattern - a plain settable-properties class needs no custom binding helper.
/// </summary>
public sealed class MongoOptions
{
    public required string Host { get; set; }
    public int Port { get; set; } = 27017;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public required string DatabaseName { get; set; }
    public string? AuthenticationDatabase { get; set; }
    public string AuthenticationMechanism { get; set; } = "SCRAM-SHA-256";
    public bool DirectConnection { get; set; } = true;

    /// <summary>
    /// If <see cref="Username"/> is empty, credentials are omitted entirely - for local/test
    /// mongod instances with auth disabled. Every caller that supplies a username also supplies
    /// a password; there is no partial-credentials case.
    /// </summary>
    public string ToConnectionString()
    {
        var builder = new MongoUrlBuilder
        {
            Server = new MongoServerAddress(Host, Port),
            DatabaseName = DatabaseName,
            DirectConnection = DirectConnection,
        };

        if (!string.IsNullOrEmpty(Username))
        {
            builder.Username = Username;
            builder.Password = Password;
            builder.AuthenticationSource = AuthenticationDatabase ?? DatabaseName;
            builder.AuthenticationMechanism = AuthenticationMechanism;
        }

        return builder.ToString();
    }
}
