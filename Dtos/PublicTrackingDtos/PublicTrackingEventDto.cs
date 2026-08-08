namespace DatabaseMastery.TransportMongoDb.Dtos.PublicTrackingDtos
{
    public class PublicTrackingEventDto
    {
        public DateTime EventDate { get; set; }

        public string TrackingStatus { get; set; } = string.Empty;
    }
}
