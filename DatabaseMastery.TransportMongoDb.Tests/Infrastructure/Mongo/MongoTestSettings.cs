using System.Text.RegularExpressions;
using DatabaseMastery.TransportMongoDb.Settings;
using MongoDB.Driver;

namespace DatabaseMastery.TransportMongoDb.Tests.Infrastructure.Mongo;

internal static class MongoTestSettings
{
    public const string DefaultConnectionString = "mongodb://127.0.0.1:27017";
    public const string IntegrationDatabasePrefix = "TransportDb_Integration_";
    public const string ShipmentCollectionName = "Shipments";
    public const string ProductionDatabaseName = "TransportDb";

    private static readonly Regex OwnedDatabaseNameRegex = new(
        "^TransportDb_Integration_[0-9a-f]{32}$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public static string GenerateOwnedDatabaseName() =>
        $"{IntegrationDatabasePrefix}{Guid.NewGuid():N}";

    public static DatabaseSettings CreateShipmentServiceSettings(string databaseName) =>
        new()
        {
            ConnectionString = DefaultConnectionString,
            DatabaseName = databaseName,
            ShipmentCollectionName = ShipmentCollectionName,
        };

    public static void ValidateOwnedDatabaseName(
        string databaseName,
        string expectedOwnedDatabaseName)
    {
        if (string.IsNullOrWhiteSpace(databaseName))
        {
            throw new InvalidOperationException("Integration database name is required.");
        }

        if (string.Equals(databaseName, ProductionDatabaseName, StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                "Integration tests must not use the production database name TransportDb.");
        }

        if (!databaseName.StartsWith(IntegrationDatabasePrefix, StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                "Integration database name must start with TransportDb_Integration_.");
        }

        if (!OwnedDatabaseNameRegex.IsMatch(databaseName))
        {
            throw new InvalidOperationException(
                "Integration database name must match TransportDb_Integration_<32-hex-guid>.");
        }

        if (!string.Equals(databaseName, expectedOwnedDatabaseName, StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                "Integration database name must be fixture-owned and immutable.");
        }
    }

    public static void ValidateLocalConnectionString(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("MongoDB connection string is required.");
        }

        if (connectionString.Contains("mongodb+srv://", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("mongodb+srv connection strings are not allowed.");
        }

        MongoUrl url;
        try
        {
            url = new MongoUrl(connectionString);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("MongoDB connection string is invalid.", ex);
        }

        var servers = url.Servers?.ToArray() ?? Array.Empty<MongoServerAddress>();
        if (servers.Length == 0 && url.Server is not null)
        {
            servers = [url.Server];
        }

        if (servers.Length == 0)
        {
            throw new InvalidOperationException("MongoDB connection string has no hosts.");
        }

        foreach (var server in servers)
        {
            ValidateAllowedHost(server.Host);
        }
    }

    public static void ValidateDropTarget(
        string targetDatabaseName,
        string ownedDatabaseName,
        string connectionString)
    {
        ValidateOwnedDatabaseName(targetDatabaseName, ownedDatabaseName);
        ValidateLocalConnectionString(connectionString);
    }

    private static void ValidateAllowedHost(string host)
    {
        if (string.Equals(host, "127.0.0.1", StringComparison.Ordinal)
            || string.Equals(host, "localhost", StringComparison.OrdinalIgnoreCase)
            || string.Equals(host, "::1", StringComparison.Ordinal))
        {
            return;
        }

        throw new InvalidOperationException(
            $"MongoDB host '{host}' is not allowed for integration tests.");
    }
}
