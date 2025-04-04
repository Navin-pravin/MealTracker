using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ProjectHierarchyApi.Models
{
    public class Location
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        [BsonElement("Name")]
        public string Name { get; set; } = string.Empty;

        public string description{get; set;}=string.Empty;

        [BsonElement("Status")]
        public bool Status { get; set; } = true; 

     
    }
}
