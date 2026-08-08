using System.Reflection;
using System.Text.Json;
using DatabaseMastery.TransportMongoDb.Dtos.PublicTrackingDtos;
using DatabaseMastery.TransportMongoDb.Services.ShipmentServices;
using DatabaseMastery.TransportMongoDb.Tests.Infrastructure.Mongo;
using MongoDB.Driver;

namespace DatabaseMastery.TransportMongoDb.Tests.MongoIntegration;

[Collection("MongoIntegration")]
[Trait("Category", "MongoIntegration")]
public sealed class ShipmentDuplicateAndProjectionTests : IClassFixture<MongoIntegrationFixture>
{
    private static readonly string[] PublicResultPropertyAllowlist =
    [
        "ArrivalCity",
        "CreatedDate",
        "CurrentStatus",
        "DepartureCity",
        "Events",
        "TrackingNumber",
    ];

    private static readonly string[] PublicEventPropertyAllowlist =
    [
        "EventDate",
        "TrackingStatus",
    ];

    private static readonly string[] PrivacyDenylistSentinels =
    [
        "PRIVATE_TEST_SENDER",
        "PRIVATE_TEST_RECEIVER",
        "PRIVATE_TEST_PHONE",
        "PRIVATE_TEST_ADDRESS",
        "PRIVATE_TEST_LOCATION",
        "PRIVATE_TEST_DESCRIPTION",
        "PRIVATE_TEST_DISTRICT",
    ];

    private readonly MongoIntegrationFixture _fixture;

    public ShipmentDuplicateAndProjectionTests(MongoIntegrationFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task UpdateToExistingTrackingNumber_ThrowsDuplicateTrackingNumberException()
    {
        var token = Guid.NewGuid().ToString("N")[..8];
        var trackingNumberA = $"TEST-INT-UPD-A-{token}";
        var trackingNumberB = $"TEST-INT-UPD-B-{token}";

        await _fixture.ShipmentService.EnsureIndexesAsync();

        await _fixture.ShipmentService.CreateShipmentAsync(
            MongoTestDataBuilder.CreateShipment(trackingNumberA));
        await _fixture.ShipmentService.CreateShipmentAsync(
            MongoTestDataBuilder.CreateShipmentWithDifferentFields(trackingNumberB));

        var shipmentB = await _fixture.ShipmentService.GetShipmentByTrackingNumberAsync(trackingNumberB);
        Assert.NotNull(shipmentB);

        var duplicateUpdate = MongoTestDataBuilder.ToUpdateShipmentDto(
            shipmentB!,
            trackingNumberA);

        var exception = await Assert.ThrowsAsync<DuplicateTrackingNumberException>(
            () => _fixture.ShipmentService.UpdateShipmentAsync(duplicateUpdate));

        Assert.Equal(
            "A shipment with the same tracking number already exists.",
            exception.Message);
        Assert.DoesNotContain(trackingNumberA, exception.Message, StringComparison.Ordinal);
        Assert.DoesNotContain(trackingNumberB, exception.Message, StringComparison.Ordinal);
        Assert.IsNotType<MongoWriteException>(exception);
    }

    [Fact]
    public async Task DuplicateTrackingNumberUpdate_DoesNotModifyExistingShipment()
    {
        var token = Guid.NewGuid().ToString("N")[..8];
        var trackingNumberA = $"TEST-INT-ATOM-A-{token}";
        var trackingNumberB = $"TEST-INT-ATOM-B-{token}";

        await _fixture.ShipmentService.EnsureIndexesAsync();

        await _fixture.ShipmentService.CreateShipmentAsync(
            MongoTestDataBuilder.CreateShipment(trackingNumberA));
        await _fixture.ShipmentService.CreateShipmentAsync(
            MongoTestDataBuilder.CreateShipmentWithDifferentFields(trackingNumberB));

        var shipmentA = await _fixture.ShipmentService.GetShipmentByTrackingNumberAsync(trackingNumberA);
        var shipmentB = await _fixture.ShipmentService.GetShipmentByTrackingNumberAsync(trackingNumberB);
        Assert.NotNull(shipmentA);
        Assert.NotNull(shipmentB);

        var duplicateUpdate = MongoTestDataBuilder.ToUpdateShipmentDto(
            shipmentB!,
            trackingNumberA);

        await Assert.ThrowsAsync<DuplicateTrackingNumberException>(
            () => _fixture.ShipmentService.UpdateShipmentAsync(duplicateUpdate));

        var shipmentAAfter = await _fixture.ShipmentService.GetShipmentByTrackingNumberAsync(trackingNumberA);
        var shipmentBAfter = await _fixture.ShipmentService.GetShipmentByTrackingNumberAsync(trackingNumberB);

        Assert.NotNull(shipmentAAfter);
        Assert.NotNull(shipmentBAfter);
        Assert.Equal(shipmentA!.TrackingNumber, shipmentAAfter!.TrackingNumber);
        Assert.Equal(shipmentA.SenderName, shipmentAAfter.SenderName);
        Assert.Equal(shipmentB!.TrackingNumber, shipmentBAfter!.TrackingNumber);
        Assert.Equal(shipmentB.SenderName, shipmentBAfter.SenderName);
        Assert.Equal("Other Test Sender", shipmentBAfter.SenderName);
    }

    [Fact]
    public async Task GetPublicTrackingByTrackingNumber_ReturnsOnlyExpectedPublicData()
    {
        var trackingNumber = $"TEST-INT-PUB-{Guid.NewGuid():N}";
        var createdDate = new DateTime(2026, 3, 4, 15, 45, 30, DateTimeKind.Utc);

        await _fixture.ShipmentService.EnsureIndexesAsync();
        await _fixture.Shipments.InsertOneAsync(
            MongoTestDataBuilder.CreateSensitiveShipmentEntity(trackingNumber, createdDate));

        var result = await _fixture.ShipmentService
            .GetPublicTrackingByTrackingNumberAsync(trackingNumber);

        Assert.NotNull(result);
        Assert.Equal(trackingNumber, result!.TrackingNumber);
        Assert.Equal("Public Status", result.CurrentStatus);
        Assert.Equal("Public Departure City", result.DepartureCity);
        Assert.Equal("Public Arrival City", result.ArrivalCity);
        Assert.Equal(createdDate, result.CreatedDate);

        var trackingEvent = Assert.Single(result.Events);
        Assert.Equal(new DateTime(2026, 2, 1, 10, 30, 0, DateTimeKind.Utc), trackingEvent.EventDate);
        Assert.Equal("Public Event Status", trackingEvent.TrackingStatus);

        AssertPropertyAllowlist(typeof(PublicTrackingResultDto), PublicResultPropertyAllowlist);
        AssertPropertyAllowlist(typeof(PublicTrackingEventDto), PublicEventPropertyAllowlist);

        var serialized = JsonSerializer.Serialize(result);
        foreach (var sentinel in PrivacyDenylistSentinels)
        {
            Assert.DoesNotContain(sentinel, serialized, StringComparison.Ordinal);
        }
    }

    [Fact]
    public async Task PublicTrackingLookup_IsCaseSensitive()
    {
        var token = Guid.NewGuid().ToString("N")[..8];
        var trackingNumber = $"LOOKUP-{token}-ABC";

        await _fixture.ShipmentService.EnsureIndexesAsync();
        await _fixture.ShipmentService.CreateShipmentAsync(
            MongoTestDataBuilder.CreateShipment(trackingNumber));

        var exactResult = await _fixture.ShipmentService
            .GetPublicTrackingByTrackingNumberAsync(trackingNumber);
        var caseChangedResult = await _fixture.ShipmentService
            .GetPublicTrackingByTrackingNumberAsync(trackingNumber.ToLowerInvariant());

        Assert.NotNull(exactResult);
        Assert.Null(caseChangedResult);
    }

    [Fact]
    public async Task GetPublicTrackingByTrackingNumber_WhenMissing_ReturnsNull()
    {
        var missingTrackingNumber = $"TEST-INT-MISSING-{Guid.NewGuid():N}";

        var result = await _fixture.ShipmentService
            .GetPublicTrackingByTrackingNumberAsync(missingTrackingNumber);

        Assert.Null(result);
    }

    private static void AssertPropertyAllowlist(Type dtoType, IReadOnlyCollection<string> expectedProperties)
    {
        var actualProperties = dtoType
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Select(property => property.Name)
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(
            expectedProperties.OrderBy(name => name, StringComparer.Ordinal).ToArray(),
            actualProperties);
    }
}
