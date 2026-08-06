namespace DatabaseMastery.TransportMongoDb.Settings
{
    public class DatabaseSettings : IDatabaseSettings   
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string SliderCollectionName { get; set; }

        public string BrandCollectionName { get; set; }

        public string OfferCollectionName { get; set; }

        public string AboutCollectionName { get; set; }
        public string GetInTouchCollectionName { get; set; }
        public string HowItWorksCollectionName { get; set; }
        public string TestimonialCollectionName { get; set; }
        public string ProjectCollectionName { get; set; } = string.Empty;
        public string ShipmentCollectionName { get; set; } = string.Empty;
        public string AdminUserCollectionName { get; set; } = string.Empty;
    }
}
