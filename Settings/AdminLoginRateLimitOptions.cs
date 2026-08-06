using System.ComponentModel.DataAnnotations;

namespace DatabaseMastery.TransportMongoDb.Settings
{
    public class AdminLoginRateLimitOptions
    {
        public const string SectionName = "AdminLoginRateLimit";

        [Range(5, 1000)]
        public int IpPermitLimit { get; set; } = 30;

        [Range(10, 3600)]
        public int IpWindowSeconds { get; set; } = 60;

        [Range(2, 100)]
        public int IdentityPermitLimit { get; set; } = 5;

        [Range(10, 3600)]
        public int IdentityWindowSeconds { get; set; } = 60;

        [Range(5, 1440)]
        public int PartitionIdleMinutes { get; set; } = 30;

        [Range(100, 100000)]
        public int MaxPartitions { get; set; } = 10000;
    }
}
