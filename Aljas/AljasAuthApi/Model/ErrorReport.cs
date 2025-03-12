using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AljasAuthApi.Models
{
    public class ErrorReport
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public int RowNumber { get; set; } // Excel Row Number
        public required string IDNumber { get; set; }
        public required string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Dept { get; set; }
        public string Role { get; set; }
        public string Designation { get; set; }
        public string Company { get; set; }
        public string Location { get; set; }
        public string Status { get; set; } // "Failed" or "Success"

        [BsonElement("Errors")]
        public List<string> ErrorMessage { get; set; }
    }
}
