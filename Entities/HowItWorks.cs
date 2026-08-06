using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DatabaseMastery.TransportMongoDb.Entities
{
    public class HowItWorks
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string HowItWorksId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string ImageUrl { get; set; }
    }
}
