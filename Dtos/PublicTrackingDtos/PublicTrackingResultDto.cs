namespace DatabaseMastery.TransportMongoDb.Dtos.PublicTrackingDtos
{
    public class PublicTrackingResultDto
    {
        public string TrackingNumber { get; set; } = string.Empty;

        public string CurrentStatus { get; set; } = string.Empty;

        public string DepartureCity { get; set; } = string.Empty;

        public string ArrivalCity { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; }

        public IReadOnlyList<PublicTrackingEventDto> Events { get; set; } =
            Array.Empty<PublicTrackingEventDto>();
    }
}
