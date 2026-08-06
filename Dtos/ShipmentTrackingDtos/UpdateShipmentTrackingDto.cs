namespace DatabaseMastery.TransportMongoDb.Dtos.ShipmentTrackingDtos
{
    public class UpdateShipmentTrackingDto
    {
        public string TrackingNumber { get; set; } = string.Empty;

        public int TrackingIndex { get; set; }

        public DateTime EventDate { get; set; }

        public string Location { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string TrackingStatus { get; set; } = string.Empty;
    }
}
