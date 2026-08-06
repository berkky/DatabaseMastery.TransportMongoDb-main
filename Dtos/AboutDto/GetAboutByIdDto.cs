namespace DatabaseMastery.TransportMongoDb.Dtos.AboutDto
{
    public class GetAboutByIdDto
    {
        public string AboutId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string ImageUrl { get; set; }
    }
}
