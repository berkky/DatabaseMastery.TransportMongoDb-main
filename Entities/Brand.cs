using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DatabaseMastery.TransportMongoDb.Entities
{
    public class Brand
    {
        [BsonId] //primary key
        [BsonRepresentation(BsonType.ObjectId)] //MongoDB ObjectId
        public string BrandId { get; set; }

        public string BrandName { get; set; }

        public string ImageUrl { get; set; }

        public bool IsStatus { get; set; }
    }
}
