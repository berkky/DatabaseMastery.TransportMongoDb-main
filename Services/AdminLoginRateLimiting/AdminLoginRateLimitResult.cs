namespace DatabaseMastery.TransportMongoDb.Services.AdminLoginRateLimiting
{
    public sealed class AdminLoginRateLimitResult
    {
        public bool IsAllowed { get; init; }

        public TimeSpan RetryAfter { get; init; }
    }
}
