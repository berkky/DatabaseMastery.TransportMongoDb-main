using System.ComponentModel.DataAnnotations;

namespace DatabaseMastery.TransportMongoDb.Settings
{
    public class PublicTrackingRateLimitOptions
    {
        public const string SectionName = "PublicTrackingRateLimit";

        [Range(5, 1000)]
        public int PermitLimit { get; set; } = 30;

        [Range(10, 3600)]
        public int WindowSeconds { get; set; } = 60;

        [Range(5, 1440)]
        public int PartitionIdleMinutes { get; set; } = 30;

        [Range(100, 100000)]
        public int MaxPartitions { get; set; } = 10000;
    }
}
