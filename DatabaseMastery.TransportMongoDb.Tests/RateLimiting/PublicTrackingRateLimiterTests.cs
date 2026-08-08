using System.Net;
using DatabaseMastery.TransportMongoDb.Services.PublicTrackingRateLimiting;
using DatabaseMastery.TransportMongoDb.Settings;
using Microsoft.Extensions.Options;

namespace DatabaseMastery.TransportMongoDb.Tests.RateLimiting;

public sealed class PublicTrackingRateLimiterTests
{
    private static PublicTrackingRateLimiter CreateLimiter(
        int permitLimit = 5,
        int windowSeconds = 10)
    {
        var options = Options.Create(new PublicTrackingRateLimitOptions
        {
            PermitLimit = permitLimit,
            WindowSeconds = windowSeconds,
            PartitionIdleMinutes = 5,
            MaxPartitions = 100
        });

        return new PublicTrackingRateLimiter(options);
    }

    [Fact]
    public async Task AllowsRequestsUpToPermitLimit()
    {
        using var limiter = CreateLimiter();
        var ip = IPAddress.Parse("203.0.113.10");

        for (var attempt = 0; attempt < 5; attempt++)
        {
            var result = await limiter.AcquireAsync(ip, CancellationToken.None);
            Assert.True(result.IsAllowed);
            Assert.Equal(TimeSpan.Zero, result.RetryAfter);
        }
    }

    [Fact]
    public async Task RejectsRequestAfterPermitLimit()
    {
        using var limiter = CreateLimiter();
        var ip = IPAddress.Parse("203.0.113.11");

        for (var attempt = 0; attempt < 5; attempt++)
        {
            await limiter.AcquireAsync(ip, CancellationToken.None);
        }

        var rejected = await limiter.AcquireAsync(ip, CancellationToken.None);

        Assert.False(rejected.IsAllowed);
    }

    [Fact]
    public async Task RejectedResultContainsPositiveRetryAfter()
    {
        using var limiter = CreateLimiter();
        var ip = IPAddress.Parse("203.0.113.12");

        for (var attempt = 0; attempt < 5; attempt++)
        {
            await limiter.AcquireAsync(ip, CancellationToken.None);
        }

        var rejected = await limiter.AcquireAsync(ip, CancellationToken.None);

        Assert.False(rejected.IsAllowed);
        Assert.True(rejected.RetryAfter > TimeSpan.Zero);
    }

    [Fact]
    public async Task DifferentIpAddressesUseIndependentPartitions()
    {
        using var limiter = CreateLimiter();
        var ipA = IPAddress.Parse("203.0.113.20");
        var ipB = IPAddress.Parse("203.0.113.21");

        for (var attempt = 0; attempt < 5; attempt++)
        {
            await limiter.AcquireAsync(ipA, CancellationToken.None);
        }

        var rejectedForA = await limiter.AcquireAsync(ipA, CancellationToken.None);
        var allowedForB = await limiter.AcquireAsync(ipB, CancellationToken.None);

        Assert.False(rejectedForA.IsAllowed);
        Assert.True(allowedForB.IsAllowed);
    }

    [Fact]
    public async Task NullIpUsesStableUnknownPartition()
    {
        using var limiter = CreateLimiter();

        for (var attempt = 0; attempt < 5; attempt++)
        {
            var allowed = await limiter.AcquireAsync(null, CancellationToken.None);
            Assert.True(allowed.IsAllowed);
        }

        var rejected = await limiter.AcquireAsync(null, CancellationToken.None);

        Assert.False(rejected.IsAllowed);
    }

    [Fact]
    public async Task QueueLimitIsZeroBehaviorRejectsImmediately()
    {
        using var limiter = CreateLimiter();
        var ip = IPAddress.Parse("203.0.113.30");

        for (var attempt = 0; attempt < 5; attempt++)
        {
            await limiter.AcquireAsync(ip, CancellationToken.None);
        }

        var rejected = await limiter.AcquireAsync(ip, CancellationToken.None);

        Assert.False(rejected.IsAllowed);
    }

    [Fact]
    public void DisposeCanBeCalledSafely()
    {
        var limiter = CreateLimiter();

        var exception = Record.Exception(() => limiter.Dispose());

        Assert.Null(exception);
    }
}
