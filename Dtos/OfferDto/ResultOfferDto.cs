namespace DatabaseMastery.TransportMongoDb.Dtos.OfferDto
{
    public class ResultOfferDto
    {
        public string OfferId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string ImageUrl { get; set; }

        public bool IsStatus { get; set; }
    }
}
