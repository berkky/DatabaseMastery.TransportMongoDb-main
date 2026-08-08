using System.Net;
using DatabaseMastery.TransportMongoDb.Tests.Infrastructure;
using Microsoft.AspNetCore.Mvc.Testing;

namespace DatabaseMastery.TransportMongoDb.Tests;

public class AntiforgeryTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public AntiforgeryTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task TrackingPost_WithoutToken_ReturnsBadRequest()
    {
        var client = _factory.CreateClient();
        using var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["trackingNumber"] = string.Empty
        });

        var response = await client.PostAsync("/Tracking/Index", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(0, _factory.ShipmentServiceSpy.PublicTrackingLookupCount);
    }

    [Fact]
    public async Task AccountLoginPost_WithoutToken_ReturnsBadRequest()
    {
        var client = _factory.CreateClient();
        using var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["Username"] = "test",
            ["Password"] = "test"
        });

        var response = await client.PostAsync("/Account/Login", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task TrackingPost_WithValidTokenAndEmptyTrackingNumber_ReturnsValidationPathWithoutLookup()
    {
        var client = _factory.CreateClient();
        var getResponse = await client.GetAsync("/Tracking/Index");
        getResponse.EnsureSuccessStatusCode();

        var html = await getResponse.Content.ReadAsStringAsync();
        var token = HttpTestHelpers.ExtractAntiforgeryToken(html);

        using var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["trackingNumber"] = string.Empty,
            ["__RequestVerificationToken"] = token
        });

        var postResponse = await client.PostAsync("/Tracking/Index", content);

        Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);
        Assert.Equal(0, _factory.ShipmentServiceSpy.PublicTrackingLookupCount);

        var body = await postResponse.Content.ReadAsStringAsync();
        Assert.Contains("takip numaran", body, StringComparison.OrdinalIgnoreCase);
    }
}
