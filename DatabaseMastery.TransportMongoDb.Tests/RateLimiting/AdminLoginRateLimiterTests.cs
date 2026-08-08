using System.Net;
using DatabaseMastery.TransportMongoDb.Services.AdminLoginRateLimiting;
using DatabaseMastery.TransportMongoDb.Settings;
using Microsoft.Extensions.Options;

namespace DatabaseMastery.TransportMongoDb.Tests.RateLimiting;

public sealed class AdminLoginRateLimiterTests
{
    private static AdminLoginRateLimiter CreateLimiter(
        int identityPermitLimit = 2,
        int identityWindowSeconds = 10)
    {
        var options = Options.Create(new AdminLoginRateLimitOptions
        {
            IdentityPermitLimit = identityPermitLimit,
            IdentityWindowSeconds = identityWindowSeconds,
            PartitionIdleMinutes = 5,
            MaxPartitions = 100
        });

        return new AdminLoginRateLimiter(options);
    }

    [Fact]
    public async Task AllowsRequestsUpToIdentityPermitLimit()
    {
        using var limiter = CreateLimiter();
        var ip = IPAddress.Parse("198.51.100.10");
        const string username = "admin.user";

        for (var attempt = 0; attempt < 2; attempt++)
        {
            var result = await limiter.AcquireAsync(username, ip, CancellationToken.None);
            Assert.True(result.IsAllowed);
            Assert.Equal(TimeSpan.Zero, result.RetryAfter);
        }
    }

    [Fact]
    public async Task RejectsIdentityAfterPermitLimit()
    {
        using var limiter = CreateLimiter();
        var ip = IPAddress.Parse("198.51.100.11");
        const string username = "admin.user";

        for (var attempt = 0; attempt < 2; attempt++)
        {
            await limiter.AcquireAsync(username, ip, CancellationToken.None);
        }

        var rejected = await limiter.AcquireAsync(username, ip, CancellationToken.None);

        Assert.False(rejected.IsAllowed);
    }

    [Fact]
    public async Task RejectedIdentityContainsPositiveRetryAfter()
    {
        using var limiter = CreateLimiter();
        var ip = IPAddress.Parse("198.51.100.12");
        const string username = "admin.user";

        for (var attempt = 0; attempt < 2; attempt++)
        {
            await limiter.AcquireAsync(username, ip, CancellationToken.None);
        }

        var rejected = await limiter.AcquireAsync(username, ip, CancellationToken.None);

        Assert.False(rejected.IsAllowed);
        Assert.True(rejected.RetryAfter > TimeSpan.Zero);
    }

    [Fact]
    public async Task DifferentUsernamesUseIndependentPartitions()
    {
        using var limiter = CreateLimiter();
        var ip = IPAddress.Parse("198.51.100.20");

        for (var attempt = 0; attempt < 2; attempt++)
        {
            await limiter.AcquireAsync("user-a", ip, CancellationToken.None);
        }

        var rejectedForUserA = await limiter.AcquireAsync("user-a", ip, CancellationToken.None);
        var allowedForUserB = await limiter.AcquireAsync("user-b", ip, CancellationToken.None);

        Assert.False(rejectedForUserA.IsAllowed);
        Assert.True(allowedForUserB.IsAllowed);
    }

    [Fact]
    public async Task DifferentIpAddressesUseIndependentPartitions()
    {
        using var limiter = CreateLimiter();
        const string username = "shared.user";
        var ipA = IPAddress.Parse("198.51.100.30");
        var ipB = IPAddress.Parse("198.51.100.31");

        for (var attempt = 0; attempt < 2; attempt++)
        {
            await limiter.AcquireAsync(username, ipA, CancellationToken.None);
        }

        var rejectedForIpA = await limiter.AcquireAsync(username, ipA, CancellationToken.None);
        var allowedForIpB = await limiter.AcquireAsync(username, ipB, CancellationToken.None);

        Assert.False(rejectedForIpA.IsAllowed);
        Assert.True(allowedForIpB.IsAllowed);
    }

    [Fact]
    public async Task UsernameNormalizationSemanticsAreRespected()
    {
        using var limiter = CreateLimiter();
        var ip = IPAddress.Parse("198.51.100.40");

        await limiter.AcquireAsync("normalized-user", ip, CancellationToken.None);
        await limiter.AcquireAsync("normalized-user", ip, CancellationToken.None);
        var rejectedSameNormalized = await limiter.AcquireAsync("normalized-user", ip, CancellationToken.None);

        var allowedDifferentCase = await limiter.AcquireAsync("Normalized-User", ip, CancellationToken.None);

        Assert.False(rejectedSameNormalized.IsAllowed);
        Assert.True(allowedDifferentCase.IsAllowed);

        await Assert.ThrowsAsync<ArgumentException>(async () =>
            await limiter.AcquireAsync("   ", ip, CancellationToken.None));
    }

    [Fact]
    public async Task NullIpUsesStablePartition()
    {
        using var limiter = CreateLimiter();
        const string username = "admin.user";

        for (var attempt = 0; attempt < 2; attempt++)
        {
            var allowed = await limiter.AcquireAsync(username, null, CancellationToken.None);
            Assert.True(allowed.IsAllowed);
        }

        var rejected = await limiter.AcquireAsync(username, null, CancellationToken.None);

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
