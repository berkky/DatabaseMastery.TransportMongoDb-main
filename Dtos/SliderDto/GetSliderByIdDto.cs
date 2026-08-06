namespace DatabaseMastery.TransportMongoDb.Dtos.SliderDto
{
    public class GetSliderByIdDto
    {
        public string SliderId { get; set; }
        public string SliderTitle { get; set; }
        public string Subtitle { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
    }
}
