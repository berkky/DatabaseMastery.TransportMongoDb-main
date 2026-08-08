using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using DatabaseMastery.TransportMongoDb.Tests.Infrastructure;
using DatabaseMastery.TransportMongoDb.Tests.Infrastructure.ReverseProxy;
using Microsoft.AspNetCore.Mvc.Testing;

namespace DatabaseMastery.TransportMongoDb.Tests.ReverseProxy;

public sealed class ForwardedHeadersTests
{
    private const string ProbePath = "/ForwardedHeadersProbe";
    private const string LoginPath = "/Account/Login";
    private const string TrackingPath = "/Tracking/Index";

    [Fact]
    public async Task TrustedLoopbackProxy_AppliesForwardedClientIp()
    {
        using var factory = new ForwardedHeadersWebApplicationFactory();
        var client = factory.CreateClient();

        var response = await SendProbeAsync(
            client,
            proxyRemoteIp: "127.0.0.1",
            forwardedFor: "198.51.100.42");

        response.EnsureSuccessStatusCode();
        var probe = await ReadProbeAsync(response);

        Assert.Equal("198.51.100.42", probe.RemoteIpAddress);
    }

    [Fact]
    public async Task TrustedLoopbackProxy_AppliesForwardedHttpsScheme()
    {
        using var factory = new ForwardedHeadersWebApplicationFactory();
        var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("http://localhost"),
            AllowAutoRedirect = false,
        });

        var response = await SendProbeAsync(
            client,
            proxyRemoteIp: "127.0.0.1",
            forwardedProto: "https");

        response.EnsureSuccessStatusCode();
        var probe = await ReadProbeAsync(response);

        Assert.Equal("https", probe.Scheme);
        Assert.True(probe.IsHttps);
    }

    [Fact]
    public async Task TrustedIpv6LoopbackProxy_AppliesForwardedClientIp()
    {
        using var factory = new ForwardedHeadersWebApplicationFactory();
        var client = factory.CreateClient();

        var response = await SendProbeAsync(
            client,
            proxyRemoteIp: "::1",
            forwardedFor: "203.0.113.10");

        response.EnsureSuccessStatusCode();
        var probe = await ReadProbeAsync(response);

        Assert.Equal("203.0.113.10", probe.RemoteIpAddress);
    }

    [Fact]
    public async Task UntrustedRemoteAddress_CannotSpoofForwardedFor()
    {
        using var factory = new ForwardedHeadersWebApplicationFactory();
        var client = factory.CreateClient();

        var response = await SendProbeAsync(
            client,
            proxyRemoteIp: "203.0.113.50",
            forwardedFor: "198.51.100.99");

        response.EnsureSuccessStatusCode();
        var probe = await ReadProbeAsync(response);

        Assert.Equal("203.0.113.50", probe.RemoteIpAddress);
    }

    [Fact]
    public async Task UntrustedRemoteAddress_CannotSpoofForwardedProto()
    {
        using var factory = new ForwardedHeadersWebApplicationFactory();
        var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("http://localhost"),
            AllowAutoRedirect = false,
        });

        var response = await SendProbeAsync(
            client,
            proxyRemoteIp: "203.0.113.50",
            forwardedProto: "https");

        response.EnsureSuccessStatusCode();
        var probe = await ReadProbeAsync(response);

        Assert.Equal("http", probe.Scheme);
        Assert.False(probe.IsHttps);
    }

    [Fact]
    public async Task TrustedLoopbackProxy_ForwardLimitBoundsForwardedForChain()
    {
        using var factory = new ForwardedHeadersWebApplicationFactory();
        var client = factory.CreateClient();

        var singleHopResponse = await SendProbeAsync(
            client,
            proxyRemoteIp: "127.0.0.1",
            forwardedFor: "198.51.100.10");
        singleHopResponse.EnsureSuccessStatusCode();
        var singleHopProbe = await ReadProbeAsync(singleHopResponse);

        var chainResponse = await SendProbeAsync(
            client,
            proxyRemoteIp: "127.0.0.1",
            forwardedFor: "198.51.100.10, 198.51.100.20");
        chainResponse.EnsureSuccessStatusCode();
        var chainProbe = await ReadProbeAsync(chainResponse);

        Assert.Equal("198.51.100.10", singleHopProbe.RemoteIpAddress);
        Assert.Equal("198.51.100.20", chainProbe.RemoteIpAddress);
        Assert.NotEqual(singleHopProbe.RemoteIpAddress, chainProbe.RemoteIpAddress);
    }

    [Fact]
    public async Task AdminLoginRateLimiter_UsesForwardedClientIpBehindTrustedProxy()
    {
        using var factory = new ForwardedHeadersWebApplicationFactory();
        var client = factory.CreateClient();
        const string clientA = "198.51.100.30";
        const string clientB = "198.51.100.31";

        for (var attempt = 0; attempt < factory.IpPermitLimit; attempt++)
        {
            await PostLoginBehindProxyAsync(client, clientA);
        }

        var rejectedForClientA = await PostLoginBehindProxyAsync(client, clientA);
        var allowedForClientB = await PostLoginBehindProxyAsync(client, clientB);

        Assert.Equal(HttpStatusCode.TooManyRequests, rejectedForClientA.StatusCode);
        Assert.NotEqual(HttpStatusCode.TooManyRequests, allowedForClientB.StatusCode);
    }

    [Fact]
    public async Task TrustedForwardedHttps_DoesNotCauseRedirectLoopOnTrackingIndex()
    {
        using var factory = new ForwardedHeadersWebApplicationFactory
        {
            UseProductionEnvironment = true,
        };

        var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("http://localhost"),
            AllowAutoRedirect = false,
        });

        using var request = new HttpRequestMessage(HttpMethod.Get, TrackingPath);
        request.Headers.Add(TestRemoteIpStartupFilter.HeaderName, "127.0.0.1");
        request.Headers.Add("X-Forwarded-Proto", "https");

        var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotEqual(HttpStatusCode.MovedPermanently, response.StatusCode);
        Assert.NotEqual(HttpStatusCode.Redirect, response.StatusCode);
        Assert.NotEqual(HttpStatusCode.TemporaryRedirect, response.StatusCode);
        Assert.NotEqual(HttpStatusCode.PermanentRedirect, response.StatusCode);
    }

    private static async Task<HttpResponseMessage> SendProbeAsync(
        HttpClient client,
        string proxyRemoteIp,
        string? forwardedFor = null,
        string? forwardedProto = null)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, ProbePath);
        request.Headers.Add(TestRemoteIpStartupFilter.HeaderName, proxyRemoteIp);

        if (forwardedFor is not null)
        {
            request.Headers.Add("X-Forwarded-For", forwardedFor);
        }

        if (forwardedProto is not null)
        {
            request.Headers.Add("X-Forwarded-Proto", forwardedProto);
        }

        return await client.SendAsync(request);
    }

    private static async Task<ForwardedHeadersProbeController.ForwardedHeadersProbeResponse> ReadProbeAsync(
        HttpResponseMessage response)
    {
        var probe = await response.Content.ReadFromJsonAsync<ForwardedHeadersProbeController.ForwardedHeadersProbeResponse>(
            new JsonSerializerOptions(JsonSerializerDefaults.Web));

        return probe ?? throw new InvalidOperationException("Probe response was empty.");
    }

    private static async Task<HttpResponseMessage> PostLoginBehindProxyAsync(
        HttpClient client,
        string forwardedClientIp)
    {
        using var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["Username"] = "test-forwarded-user",
            ["Password"] = "test-forwarded-password",
        });

        using var request = new HttpRequestMessage(HttpMethod.Post, LoginPath)
        {
            Content = content,
        };

        request.Headers.Add(TestRemoteIpStartupFilter.HeaderName, "127.0.0.1");
        request.Headers.Add("X-Forwarded-For", forwardedClientIp);

        return await client.SendAsync(request);
    }
}
