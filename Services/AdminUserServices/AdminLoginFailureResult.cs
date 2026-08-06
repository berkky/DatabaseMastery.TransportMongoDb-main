namespace DatabaseMastery.TransportMongoDb.Services.AdminUserServices
{
    public sealed class AdminLoginFailureResult
    {
        public int FailedLoginCount { get; init; }

        public DateTime? LockoutEndUtc { get; init; }

        public bool IsLockedOut { get; init; }
    }
}
