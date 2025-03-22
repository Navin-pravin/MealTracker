using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ProjectHierarchyApi.Models
{
    public class Canteen
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        [BsonElement("Name")]
        public string Name { get; set; } = string.Empty;

       // [BsonElement("ProjectId")]
        //[BsonRepresentation(BsonType.ObjectId)] // Ensures compatibility with MongoDB ObjectId
        //public string ProjectId { get; set; } = string.Empty;

        [BsonElement("LocationId")]
        [BsonRepresentation(BsonType.ObjectId)] // Ensures compatibility with MongoDB ObjectId
        public string LocationId { get; set; } = string.Empty;
    }
}
