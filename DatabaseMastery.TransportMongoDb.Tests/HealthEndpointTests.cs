using System.Net;
using System.Text.Json;
using DatabaseMastery.TransportMongoDb.Tests.Infrastructure;

namespace DatabaseMastery.TransportMongoDb.Tests;

public class HealthEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public HealthEndpointTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task HealthLive_ReturnsHealthyMinimalResponse()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/health/live");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        HttpTestHelpers.AssertNoStore(response);
        HttpTestHelpers.AssertBaselineSecurityHeaders(response);
        HttpTestHelpers.AssertCorrelationId(response);

        using var document = await HttpTestHelpers.ReadJsonAsync(response);
        HttpTestHelpers.AssertMinimalHealthBody(document, "Healthy");
    }

    [Fact]
    public async Task HealthReady_ReturnsHealthyMinimalResponse()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/health/ready");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        HttpTestHelpers.AssertNoStore(response);
        HttpTestHelpers.AssertBaselineSecurityHeaders(response);

        var body = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(body);
        HttpTestHelpers.AssertMinimalHealthBody(document, "Healthy");
        Assert.DoesNotContain("mongodb://", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("ConnectionString", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("DatabaseName", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("exception", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("stack", body, StringComparison.OrdinalIgnoreCase);
    }
}

public class HealthEndpointUnhealthyTests : IClassFixture<UnhealthyReadyWebApplicationFactory>
{
    private readonly UnhealthyReadyWebApplicationFactory _factory;

    public HealthEndpointUnhealthyTests(UnhealthyReadyWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task HealthReady_ReturnsUnhealthyMinimalResponse()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/health/ready");

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
        HttpTestHelpers.AssertNoStore(response);
        HttpTestHelpers.AssertBaselineSecurityHeaders(response);
        HttpTestHelpers.AssertCorrelationId(response);

        var body = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(body);
        HttpTestHelpers.AssertMinimalHealthBody(document, "Unhealthy");
        Assert.DoesNotContain("mongodb://", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("ConnectionString", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("DatabaseName", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("exception", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("stack", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("localhost", body, StringComparison.OrdinalIgnoreCase);
    }
}
