using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace AljasAuthApi.Models
{
    public class EmployeeUploadError
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public int RowNumber { get; set; }

        // Ensure this references Employee model correctly
        public Employee EmployeeData { get; set; }

        public Dictionary<string, string> Errors { get; set; } = new Dictionary<string, string>();
    }
}
