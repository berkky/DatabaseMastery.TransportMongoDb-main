using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
namespace DatabaseMastery.TransportMongoDb.Entities
{
    public class Slider
    {
        [BsonId] //primary key
        [BsonRepresentation(BsonType.ObjectId)] //MongoDB ObjectId
        public string SliderId { get; set; }
        public string SliderTitle { get; set; }
        public string Subtitle { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
    }
}
