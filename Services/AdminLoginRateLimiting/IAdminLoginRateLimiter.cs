using System.Net;

namespace DatabaseMastery.TransportMongoDb.Services.AdminLoginRateLimiting
{
    public interface IAdminLoginRateLimiter
    {
        ValueTask<AdminLoginRateLimitResult> AcquireAsync(
            string normalizedUsername,
            IPAddress? remoteIpAddress,
            CancellationToken cancellationToken);
    }
}
