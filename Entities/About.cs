using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace DatabaseMastery.TransportMongoDb.Entities
{
    public class About
    {
        [BsonId] //primary key
        [BsonRepresentation(BsonType.ObjectId)] //MongoDB ObjectId

        public string AboutId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string ImageUrl { get; set; }
    }
}
