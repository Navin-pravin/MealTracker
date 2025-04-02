using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace AljasAuthApi.Models
{
     public class Department
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)] // ✅ Store Id as ObjectId
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString(); // ✅ Generate new ObjectId as a string

        [BsonElement("DepartmentName")]
        public required string DepartmentName { get; set; }

        [BsonElement("Description")]
        public required string Description { get; set; }
    }



    public class Company
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public required string Id { get; set; } 
        public required string CompanyName { get; set; }
        public required string description{ get; set; }
    }

    public class Designation
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public required string Id { get; set; } 
        public required string DesignationTitle { get; set; }
        public required string description { get; set; }
        
    }
    public class CLocation
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public required string Id { get; set; } 
        public required string location { get; set; }
        public required string description { get; set; }
    }
  public class MealConfiguration
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public required string Id { get; set; }

    [BsonElement("MealType")]
    public required string MealType { get; set; }

    public required string description {get; set;}

    [BsonElement("StartTime")]
    [BsonIgnoreIfNull] // Ignore if null
    public TimeSpan? StartTime { get; set; } // Nullable TimeSpan

    [BsonElement("EndTime")]
    [BsonIgnoreIfNull] // Ignore if null
    public TimeSpan? EndTime { get; set; } // Nullable TimeSpan

    [BsonElement("IsActive")]
    public bool IsActive { get; set; } = true;
}




}