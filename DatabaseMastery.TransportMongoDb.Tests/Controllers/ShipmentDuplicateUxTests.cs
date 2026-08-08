using System.Net;
using DatabaseMastery.TransportMongoDb.Tests.Infrastructure;
using MongoDB.Bson;

namespace DatabaseMastery.TransportMongoDb.Tests.Controllers;

public sealed class ShipmentDuplicateUxTests
{
    private const string DuplicateMessage = "Bu takip numarası zaten kullanılıyor.";

    [Fact]
    public async Task CreateShipmentPost_WhenServiceThrowsDuplicate_ReturnsValidationMessage()
    {
        using var factory = new DuplicateShipmentUxWebApplicationFactory();
        factory.DuplicateShipmentService.Mode = DuplicateThrowMode.Create;

        var client = factory.CreateClient();
        var getResponse = await client.GetAsync("/Shipment/CreateShipment");
        getResponse.EnsureSuccessStatusCode();

        var html = await getResponse.Content.ReadAsStringAsync();
        var token = HttpTestHelpers.ExtractAntiforgeryToken(html);

        using var content = new FormUrlEncodedContent(CreateValidCreateShipmentForm(token));

        var postResponse = await client.PostAsync("/Shipment/CreateShipment", content);
        var body = WebUtility.HtmlDecode(await postResponse.Content.ReadAsStringAsync());

        Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);
        Assert.Equal(1, factory.DuplicateShipmentService.CreateInvocationCount);
        Assert.Contains(DuplicateMessage, body, StringComparison.Ordinal);
        Assert.Equal(0, factory.DuplicateShipmentService.UpdateInvocationCount);
    }

    [Fact]
    public async Task UpdateShipmentPost_WhenServiceThrowsDuplicate_ReturnsValidationMessage()
    {
        using var factory = new DuplicateShipmentUxWebApplicationFactory();
        factory.DuplicateShipmentService.Mode = DuplicateThrowMode.Update;

        var shipmentId = ObjectId.GenerateNewId().ToString();
        var client = factory.CreateClient();

        var getResponse = await client.GetAsync($"/Shipment/UpdateShipment?id={shipmentId}");
        getResponse.EnsureSuccessStatusCode();

        var html = await getResponse.Content.ReadAsStringAsync();
        var token = HttpTestHelpers.ExtractAntiforgeryToken(html);

        using var postContent = new FormUrlEncodedContent(CreateValidUpdateShipmentForm(shipmentId, token));

        var postResponse = await client.PostAsync(
            $"/Shipment/UpdateShipment?id={shipmentId}",
            postContent);
        var body = WebUtility.HtmlDecode(await postResponse.Content.ReadAsStringAsync());

        Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);
        Assert.Equal(1, factory.DuplicateShipmentService.UpdateInvocationCount);
        Assert.Contains(DuplicateMessage, body, StringComparison.Ordinal);
    }

    private static Dictionary<string, string> CreateValidCreateShipmentForm(string antiforgeryToken) =>
        new()
        {
            ["TrackingNumber"] = "TEST-UX-CREATE-001",
            ["SenderName"] = "Test Sender",
            ["SenderPhone"] = "+905551234567",
            ["ReceiverName"] = "Test Receiver",
            ["ReceiverPhone"] = "+905551234568",
            ["DepartureCity"] = "Test City",
            ["DepartureDistrict"] = "Test District",
            ["ArrivalCity"] = "Test City",
            ["ArrivalDistrict"] = "Test District",
            ["Address"] = "Test Address",
            ["CreatedDate"] = "2026-01-15T12:00",
            ["CurrentStatus"] = "Dağıtımda",
            ["__RequestVerificationToken"] = antiforgeryToken,
        };

    private static Dictionary<string, string> CreateValidUpdateShipmentForm(
        string shipmentId,
        string? token) =>
        new()
        {
            ["ShipmentId"] = shipmentId,
            ["TrackingNumber"] = "TEST-UX-UPDATE-001",
            ["SenderName"] = "Test Sender",
            ["SenderPhone"] = "+905551234567",
            ["ReceiverName"] = "Test Receiver",
            ["ReceiverPhone"] = "+905551234568",
            ["DepartureCity"] = "Test City",
            ["DepartureDistrict"] = "Test District",
            ["ArrivalCity"] = "Test City",
            ["ArrivalDistrict"] = "Test District",
            ["Address"] = "Test Address",
            ["CreatedDate"] = "2026-01-15T12:00",
            ["CurrentStatus"] = "Dağıtımda",
            ["__RequestVerificationToken"] = token ?? string.Empty,
        };
}
