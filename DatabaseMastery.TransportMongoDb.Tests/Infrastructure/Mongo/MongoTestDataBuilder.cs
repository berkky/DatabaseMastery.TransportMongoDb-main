using DatabaseMastery.TransportMongoDb.Dtos.ShipmentDtos;

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
}
