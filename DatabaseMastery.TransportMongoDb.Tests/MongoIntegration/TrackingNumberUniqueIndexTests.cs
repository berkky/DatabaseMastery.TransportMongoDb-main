using DatabaseMastery.TransportMongoDb.Entities;
using DatabaseMastery.TransportMongoDb.Services.ShipmentServices;
using DatabaseMastery.TransportMongoDb.Tests.Infrastructure.Mongo;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DatabaseMastery.TransportMongoDb.Tests.MongoIntegration;

[Collection("MongoIntegration")]
[Trait("Category", "MongoIntegration")]
public sealed class TrackingNumberUniqueIndexTests : IClassFixture<MongoIntegrationFixture>
{
    private readonly MongoIntegrationFixture _fixture;

    public TrackingNumberUniqueIndexTests(MongoIntegrationFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task EnsureIndexesAsync_CreatesExpectedUniqueIndex()
    {
        await _fixture.ShipmentService.EnsureIndexesAsync();

        var index = await GetTrackingNumberUniqueIndexAsync();

        Assert.Equal(ShipmentService.TrackingNumberUniqueIndexName, index["name"].AsString);
        Assert.Equal(new BsonDocument("TrackingNumber", 1), index["key"].AsBsonDocument);
        Assert.True(index["unique"].AsBoolean);
        Assert.False(index.Contains("collation"));
        Assert.False(index.GetValue("sparse", false).ToBoolean());
        Assert.False(index.Contains("partialFilterExpression"));
    }

    [Fact]
    public async Task EnsureIndexesAsync_IsIdempotent()
    {
        await _fixture.ShipmentService.EnsureIndexesAsync();
        await _fixture.ShipmentService.EnsureIndexesAsync();

        var matchingIndexes = await ListTrackingNumberUniqueIndexesAsync();

        Assert.Single(matchingIndexes);
    }

    [Fact]
    public async Task ExactDuplicateTrackingNumber_ThrowsDuplicateTrackingNumberException()
    {
        const string trackingNumber = "TEST-INT-DUP-001";

        await _fixture.ShipmentService.EnsureIndexesAsync();

        await _fixture.ShipmentService.CreateShipmentAsync(
            MongoTestDataBuilder.CreateShipment(trackingNumber));

        var duplicateDto = MongoTestDataBuilder.CreateShipmentWithDifferentFields(trackingNumber);

        var exception = await Assert.ThrowsAsync<DuplicateTrackingNumberException>(
            () => _fixture.ShipmentService.CreateShipmentAsync(duplicateDto));

        Assert.Equal(
            "A shipment with the same tracking number already exists.",
            exception.Message);
        Assert.DoesNotContain(trackingNumber, exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task CaseSensitiveTrackingNumbers_AreBothAccepted()
    {
        var token = Guid.NewGuid().ToString("N")[..8];
        var upperTrackingNumber = $"CASE-{token}-UP";
        var lowerTrackingNumber = $"case-{token}-up";

        await _fixture.ShipmentService.EnsureIndexesAsync();

        await _fixture.ShipmentService.CreateShipmentAsync(
            MongoTestDataBuilder.CreateShipment(upperTrackingNumber));
        await _fixture.ShipmentService.CreateShipmentAsync(
            MongoTestDataBuilder.CreateShipment(lowerTrackingNumber));

        var upperCount = await _fixture.Shipments.CountDocumentsAsync(
            Builders<Shipment>.Filter.Eq(
                shipment => shipment.TrackingNumber,
                upperTrackingNumber));

        var lowerCount = await _fixture.Shipments.CountDocumentsAsync(
            Builders<Shipment>.Filter.Eq(
                shipment => shipment.TrackingNumber,
                lowerTrackingNumber));

        Assert.Equal(1, upperCount);
        Assert.Equal(1, lowerCount);
    }

    [Fact]
    public void Fixture_OwnedDatabaseName_MatchesExpectedFormat()
    {
        Assert.Matches(
            "^TransportDb_Integration_[0-9a-f]{32}$",
            _fixture.OwnedDatabaseName);
        Assert.NotEqual(MongoTestSettings.ProductionDatabaseName, _fixture.OwnedDatabaseName);
    }

    private async Task<BsonDocument> GetTrackingNumberUniqueIndexAsync()
    {
        var indexes = await ListTrackingNumberUniqueIndexesAsync();
        return Assert.Single(indexes);
    }

    private async Task<List<BsonDocument>> ListTrackingNumberUniqueIndexesAsync()
    {
        using var cursor = await _fixture.Shipments.Indexes.ListAsync();
        var indexes = await cursor.ToListAsync();

        return indexes
            .Where(index => index["name"].AsString == ShipmentService.TrackingNumberUniqueIndexName)
            .ToList();
    }
}
