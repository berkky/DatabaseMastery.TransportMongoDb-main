using System.ComponentModel.DataAnnotations;

namespace DatabaseMastery.TransportMongoDb.Settings
{
    public class AdminLoginSecurityOptions
    {
        public const string SectionName = "AdminLoginSecurity";

        [Range(3, 20)]
        public int MaxFailedAttempts { get; set; } = 5;

        [Range(1, 1440)]
        public int LockoutMinutes { get; set; } = 15;
    }
}
