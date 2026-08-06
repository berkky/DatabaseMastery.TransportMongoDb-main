using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DatabaseMastery.TransportMongoDb.Entities
{
    public class Shipment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ShipmentId { get; set; }

        public string TrackingNumber { get; set; }

        public string SenderName { get; set; }

        public string SenderPhone { get; set; }

        public string ReceiverName { get; set; }

        public string ReceiverPhone { get; set; }

        public string DepartureCity { get; set; }

        public string DepartureDistrict { get; set; }

        public string ArrivalCity { get; set; }

        public string ArrivalDistrict { get; set; }

        public string Address { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CurrentStatus { get; set; }

        public List<ShipmentTracking> Trackings { get; set; } = new();
    }
}
