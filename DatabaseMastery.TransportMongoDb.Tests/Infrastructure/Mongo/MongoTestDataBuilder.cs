using DatabaseMastery.TransportMongoDb.Dtos.ShipmentDtos;
using DatabaseMastery.TransportMongoDb.Entities;

namespace DatabaseMastery.TransportMongoDb.Tests.Infrastructure.Mongo;

internal static class MongoTestDataBuilder
{
    private static readonly DateTime DefaultCreatedDate =
        new(2026, 1, 15, 12, 0, 0, DateTimeKind.Utc);

    public static CreateShipmentDto CreateShipment(string trackingNumber) =>
        new()
        {
            TrackingNumber = trackingNumber,
            SenderName = "Test Sender",
            SenderPhone = "5550000001",
            ReceiverName = "Test Receiver",
            ReceiverPhone = "5550000002",
            DepartureCity = "Test City",
            DepartureDistrict = "Test District",
            ArrivalCity = "Test City",
            ArrivalDistrict = "Test District",
            Address = "Test Address",
            CreatedDate = DefaultCreatedDate,
            CurrentStatus = "Dağıtımda",
        };

    public static CreateShipmentDto CreateShipmentWithDifferentFields(string trackingNumber) =>
        new()
        {
            TrackingNumber = trackingNumber,
            SenderName = "Other Test Sender",
            SenderPhone = "5550000003",
            ReceiverName = "Other Test Receiver",
            ReceiverPhone = "5550000004",
            DepartureCity = "Other Test City",
            DepartureDistrict = "Other Test District",
            ArrivalCity = "Other Test City",
            ArrivalDistrict = "Other Test District",
            Address = "Other Test Address",
            CreatedDate = DefaultCreatedDate.AddDays(1),
            CurrentStatus = "Hazırlanıyor",
        };

    public static UpdateShipmentDto ToUpdateShipmentDto(
        GetShipmentByIdDto source,
        string? trackingNumberOverride = null) =>
        new()
        {
            ShipmentId = source.ShipmentId,
            TrackingNumber = trackingNumberOverride ?? source.TrackingNumber,
            SenderName = source.SenderName,
            SenderPhone = source.SenderPhone,
            ReceiverName = source.ReceiverName,
            ReceiverPhone = source.ReceiverPhone,
            DepartureCity = source.DepartureCity,
            DepartureDistrict = source.DepartureDistrict,
            ArrivalCity = source.ArrivalCity,
            ArrivalDistrict = source.ArrivalDistrict,
            Address = source.Address,
            CreatedDate = source.CreatedDate,
            CurrentStatus = source.CurrentStatus,
        };

    public static Shipment CreateSensitiveShipmentEntity(
        string trackingNumber,
        DateTime createdDate) =>
        new()
        {
            TrackingNumber = trackingNumber,
            SenderName = "PRIVATE_TEST_SENDER",
            SenderPhone = "PRIVATE_TEST_PHONE",
            ReceiverName = "PRIVATE_TEST_RECEIVER",
            ReceiverPhone = "PRIVATE_TEST_PHONE",
            DepartureCity = "Public Departure City",
            DepartureDistrict = "PRIVATE_TEST_DISTRICT",
            ArrivalCity = "Public Arrival City",
            ArrivalDistrict = "PRIVATE_TEST_DISTRICT",
            Address = "PRIVATE_TEST_ADDRESS",
            CreatedDate = createdDate,
            CurrentStatus = "Public Status",
            Trackings =
            [
                new ShipmentTracking
                {
                    EventDate = new DateTime(2026, 2, 1, 10, 30, 0, DateTimeKind.Utc),
                    TrackingStatus = "Public Event Status",
                    Location = "PRIVATE_TEST_LOCATION",
                    Description = "PRIVATE_TEST_DESCRIPTION",
                },
            ],
        };
}
