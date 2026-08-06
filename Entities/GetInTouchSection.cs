namespace DatabaseMastery.TransportMongoDb.Entities
{
    public class GetInTouchSection
    {
        [MongoDB.Bson.Serialization.Attributes.BsonId]
        [MongoDB.Bson.Serialization.Attributes.BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string GetInTouchSectionId { get; set; }

        public string BadgeTitle { get; set; }
        public string MainTitle { get; set; }
        public string Description { get; set; }

        public string Feature1Title { get; set; }
        public string Feature1Description { get; set; }

        public string Feature2Title { get; set; }
        public string Feature2Description { get; set; }

        public string ImageUrl { get; set; }

        public bool Status { get; set; }
    }
}
