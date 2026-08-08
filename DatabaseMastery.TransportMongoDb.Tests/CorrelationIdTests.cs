using DatabaseMastery.TransportMongoDb.Tests.Infrastructure;

namespace DatabaseMastery.TransportMongoDb.Tests;

public class CorrelationIdTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public CorrelationIdTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Root_ReturnsServerGeneratedCorrelationId()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/");

        response.EnsureSuccessStatusCode();
        HttpTestHelpers.AssertCorrelationId(response);
    }

    [Fact]
    public async Task Root_DoesNotEchoClientSuppliedCorrelationId()
    {
        const string clientCorrelationId = "client-controlled-value";

        var client = _factory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Get, "/");
        request.Headers.Add("X-Correlation-ID", clientCorrelationId);

        var response = await client.SendAsync(request);

        response.EnsureSuccessStatusCode();
        HttpTestHelpers.AssertCorrelationId(response);

        Assert.True(response.Headers.TryGetValues("X-Correlation-ID", out var values));
        var responseCorrelationId = Assert.Single(values!.ToArray());
        Assert.NotEqual(clientCorrelationId, responseCorrelationId);
    }
}
