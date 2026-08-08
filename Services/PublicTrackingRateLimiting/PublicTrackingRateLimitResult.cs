namespace DatabaseMastery.TransportMongoDb.Services.PublicTrackingRateLimiting
{
    public sealed class PublicTrackingRateLimitResult
    {
        public bool IsAllowed { get; init; }

        public TimeSpan RetryAfter { get; init; }
    }
}
