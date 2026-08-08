using DatabaseMastery.TransportMongoDb.Tests.Infrastructure.Mongo;

namespace DatabaseMastery.TransportMongoDb.Tests.MongoIntegration;

public sealed class MongoTestSettingsSafetyTests
{
    [Fact]
    public void ValidateOwnedDatabaseName_RejectsTransportDb()
    {
        var exception = Assert.Throws<InvalidOperationException>(
            () => MongoTestSettings.ValidateOwnedDatabaseName(
                MongoTestSettings.ProductionDatabaseName,
                MongoTestSettings.ProductionDatabaseName));

        Assert.Contains("TransportDb", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void ValidateOwnedDatabaseName_RejectsPrefixOnlyName()
    {
        var exception = Assert.Throws<InvalidOperationException>(
            () => MongoTestSettings.ValidateOwnedDatabaseName(
                MongoTestSettings.IntegrationDatabasePrefix,
                MongoTestSettings.IntegrationDatabasePrefix));

        Assert.Contains("32-hex-guid", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void ValidateOwnedDatabaseName_RejectsMalformedSuffix()
    {
        var exception = Assert.Throws<InvalidOperationException>(
            () => MongoTestSettings.ValidateOwnedDatabaseName(
                "TransportDb_Integration_not-a-valid-guid",
                "TransportDb_Integration_not-a-valid-guid"));

        Assert.Contains("32-hex-guid", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void ValidateDropTarget_RejectsDifferentValidLookingDatabase()
    {
        var ownedDatabaseName = MongoTestSettings.GenerateOwnedDatabaseName();
        var otherDatabaseName = MongoTestSettings.GenerateOwnedDatabaseName();

        var exception = Assert.Throws<InvalidOperationException>(
            () => MongoTestSettings.ValidateDropTarget(
                otherDatabaseName,
                ownedDatabaseName,
                MongoTestSettings.DefaultConnectionString));

        Assert.Contains("fixture-owned", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void ValidateDropTarget_AcceptsExactOwnedDatabase()
    {
        var ownedDatabaseName = MongoTestSettings.GenerateOwnedDatabaseName();

        var exception = Record.Exception(
            () => MongoTestSettings.ValidateDropTarget(
                ownedDatabaseName,
                ownedDatabaseName,
                MongoTestSettings.DefaultConnectionString));

        Assert.Null(exception);
    }

    [Fact]
    public void ValidateLocalConnectionString_RejectsSrvConnection()
    {
        var exception = Assert.Throws<InvalidOperationException>(
            () => MongoTestSettings.ValidateLocalConnectionString(
                "mongodb+srv://cluster.example.com/"));

        Assert.Contains("mongodb+srv", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void ValidateLocalConnectionString_RejectsRemoteHost()
    {
        var exception = Assert.Throws<InvalidOperationException>(
            () => MongoTestSettings.ValidateLocalConnectionString(
                "mongodb://cluster.example.com:27017"));

        Assert.Contains("not allowed", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void ValidateLocalConnectionString_RejectsEvilLocalhostHostname()
    {
        var exception = Assert.Throws<InvalidOperationException>(
            () => MongoTestSettings.ValidateLocalConnectionString(
                "mongodb://evil-localhost.example.com:27017"));

        Assert.Contains("not allowed", exception.Message, StringComparison.Ordinal);
    }
}
