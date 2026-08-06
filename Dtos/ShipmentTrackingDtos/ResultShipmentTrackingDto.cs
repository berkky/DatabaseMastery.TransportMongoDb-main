namespace DatabaseMastery.TransportMongoDb.Dtos.ShipmentTrackingDtos
{
    public class ResultShipmentTrackingDto
    {
        public DateTime EventDate { get; set; }

        public string Location { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string TrackingStatus { get; set; } = string.Empty;
    }
}
