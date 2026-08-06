namespace DatabaseMastery.TransportMongoDb.Settings
{
    public class AdminBootstrapOptions
    {
        public const string SectionName = "AdminBootstrap";

        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
}
