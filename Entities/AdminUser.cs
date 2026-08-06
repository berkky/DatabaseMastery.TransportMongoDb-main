using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DatabaseMastery.TransportMongoDb.Entities
{
    public class AdminUser
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string AdminUserId { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;

        public string NormalizedUsername { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public bool IsActive { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public DateTime UpdatedAtUtc { get; set; }

        public DateTime? LastLoginAtUtc { get; set; }

        public int FailedLoginCount { get; set; }

        public DateTime? LockoutEndUtc { get; set; }
    }
}
