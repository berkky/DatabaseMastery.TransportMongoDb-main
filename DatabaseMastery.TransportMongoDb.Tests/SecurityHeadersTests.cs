using System.Net;
using DatabaseMastery.TransportMongoDb.Tests.Infrastructure;
using Microsoft.AspNetCore.Mvc.Testing;

namespace DatabaseMastery.TransportMongoDb.Tests;

public class SecurityHeadersTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public SecurityHeadersTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Root_ReturnsBaselineSecurityHeadersAndCorrelationId()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        HttpTestHelpers.AssertBaselineSecurityHeaders(response);
        HttpTestHelpers.AssertSingleHeader(response, "Referrer-Policy", "strict-origin-when-cross-origin");
        HttpTestHelpers.AssertCorrelationId(response);
    }

    [Fact]
    public async Task AccountLogin_ReturnsDenyFrameOptionsAndNoStore()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/Account/Login");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        HttpTestHelpers.AssertBaselineSecurityHeaders(response);
        HttpTestHelpers.AssertHeaderAbsent(response, "X-Frame-Options", "SAMEORIGIN");
        HttpTestHelpers.AssertNoStore(response);
    }

    [Fact]
    public async Task TrackingIndex_ReturnsDenyNoReferrerAndNoStore()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/Tracking/Index");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        HttpTestHelpers.AssertBaselineSecurityHeaders(response);
        HttpTestHelpers.AssertSingleHeader(response, "Referrer-Policy", "no-referrer");
        HttpTestHelpers.AssertNoStore(response);
        HttpTestHelpers.AssertCorrelationId(response);
        Assert.Equal(0, _factory.ShipmentServiceSpy.PublicTrackingLookupCount);
    }

    [Fact]
    public async Task AdminLayoutIndex_RedirectsToLoginWithSecurityHeaders()
    {
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        var response = await client.GetAsync("/AdminLayout/Index");

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
        Assert.Contains("/Account/Login", response.Headers.Location!.OriginalString, StringComparison.Ordinal);
        HttpTestHelpers.AssertBaselineSecurityHeaders(response);
    }
}
