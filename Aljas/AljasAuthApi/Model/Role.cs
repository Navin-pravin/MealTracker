using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AljasAuthApi.Models
{
    public class Role
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        [BsonElement("RoleId")]
        public string RoleId { get; set; } = string.Empty;

        [BsonElement("RoleName")]
        public string RoleName { get; set; } = string.Empty;
    }
}
