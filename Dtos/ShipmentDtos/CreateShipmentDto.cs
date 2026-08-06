namespace DatabaseMastery.TransportMongoDb.Dtos.ShipmentDtos
{
    public class CreateShipmentDto
    {
        public string TrackingNumber { get; set; } = string.Empty;

        public string SenderName { get; set; } = string.Empty;

        public string SenderPhone { get; set; } = string.Empty;

        public string ReceiverName { get; set; } = string.Empty;

        public string ReceiverPhone { get; set; } = string.Empty;

        public string DepartureCity { get; set; } = string.Empty;

        public string DepartureDistrict { get; set; } = string.Empty;

        public string ArrivalCity { get; set; } = string.Empty;

        public string ArrivalDistrict { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; }

        public string CurrentStatus { get; set; } = string.Empty;
    }
}
