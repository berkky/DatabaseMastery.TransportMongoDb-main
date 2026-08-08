using System.Net;
using DatabaseMastery.TransportMongoDb.Tests.Infrastructure;

namespace DatabaseMastery.TransportMongoDb.Tests.RateLimiting;

public sealed class AdminLoginIpRateLimitHttpTests
{
    private const string LoginPath = "/Account/Login";

    [Fact]
    public async Task LoginPost_AllowsRequestsUpToIpPermitLimit()
    {
        using var factory = new LoginIpRateLimitWebApplicationFactory();
        var client = factory.CreateClient();

        for (var attempt = 0; attempt < factory.IpPermitLimit; attempt++)
        {
            var response = await PostLoginAsync(client);

            Assert.NotEqual(HttpStatusCode.TooManyRequests, response.StatusCode);
        }
    }

    [Fact]
    public async Task LoginPost_ExceedingIpPermitLimit_ReturnsTooManyRequests()
    {
        using var factory = new LoginIpRateLimitWebApplicationFactory();
        var client = factory.CreateClient();

        for (var attempt = 0; attempt < factory.IpPermitLimit; attempt++)
        {
            await PostLoginAsync(client);
        }

        var rejected = await PostLoginAsync(client);

        Assert.Equal(HttpStatusCode.TooManyRequests, rejected.StatusCode);
    }

    [Fact]
    public async Task LoginPost_RateLimitRejection_ReturnsPositiveRetryAfter()
    {
        using var factory = new LoginIpRateLimitWebApplicationFactory();
        var client = factory.CreateClient();

        for (var attempt = 0; attempt < factory.IpPermitLimit; attempt++)
        {
            await PostLoginAsync(client);
        }

        var rejected = await PostLoginAsync(client);

        Assert.Equal(HttpStatusCode.TooManyRequests, rejected.StatusCode);
        Assert.True(
            rejected.Headers.TryGetValues("Retry-After", out var values),
            "Expected Retry-After response header.");

        var retryAfter = int.Parse(Assert.Single(values!.ToArray()));
        Assert.True(retryAfter > 0);
    }

    [Fact]
    public async Task LoginGet_IsNotAffectedByPostIpRateLimit()
    {
        using var factory = new LoginIpRateLimitWebApplicationFactory();
        var client = factory.CreateClient();

        for (var attempt = 0; attempt < factory.IpPermitLimit; attempt++)
        {
            await PostLoginAsync(client);
        }

        var rejected = await PostLoginAsync(client);
        Assert.Equal(HttpStatusCode.TooManyRequests, rejected.StatusCode);

        var getResponse = await client.GetAsync(LoginPath);

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        Assert.NotEqual(HttpStatusCode.TooManyRequests, getResponse.StatusCode);
    }

    [Fact]
    public async Task LoginPost_DifferentRemoteIpUsesIndependentPartition()
    {
        using var factory = new LoginIpRateLimitWebApplicationFactory();
        var client = factory.CreateClient();
        const string ipA = "198.51.100.10";
        const string ipB = "198.51.100.11";

        for (var attempt = 0; attempt < factory.IpPermitLimit; attempt++)
        {
            await PostLoginAsync(client, ipA);
        }

        var rejectedForIpA = await PostLoginAsync(client, ipA);
        var allowedForIpB = await PostLoginAsync(client, ipB);

        Assert.Equal(HttpStatusCode.TooManyRequests, rejectedForIpA.StatusCode);
        Assert.NotEqual(HttpStatusCode.TooManyRequests, allowedForIpB.StatusCode);
    }

    private static async Task<HttpResponseMessage> PostLoginAsync(
        HttpClient client,
        string? remoteIp = null)
    {
        using var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["Username"] = "test-rate-limit-user",
            ["Password"] = "test-rate-limit-password",
        });

        var request = new HttpRequestMessage(HttpMethod.Post, LoginPath)
        {
            Content = content,
        };

        if (remoteIp is not null)
        {
            request.Headers.Add(TestRemoteIpStartupFilter.HeaderName, remoteIp);
        }

        return await client.SendAsync(request);
    }
}
