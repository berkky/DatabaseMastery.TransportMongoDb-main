using System.Net;

namespace DatabaseMastery.TransportMongoDb.Services.PublicTrackingRateLimiting
{
    public interface IPublicTrackingRateLimiter
    {
        ValueTask<PublicTrackingRateLimitResult> AcquireAsync(
            IPAddress? remoteIpAddress,
            CancellationToken cancellationToken);
    }
}
