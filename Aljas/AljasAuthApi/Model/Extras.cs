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

   public class Role
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public required string Id { get; set; }

    public required string RoleTitle { get; set; }
    public required string Description { get; set; }

    public Dictionary<string, CanteenAccess> CanteenMealAccess { get; set; } = new();
}

// ✅ Model for Canteen Access
public class CanteenAccess
{
    public bool HasAccess { get; set; } = false;  // True if the role has access to this canteen
    public Dictionary<string, bool> MealAccess { get; set; } = new(); // Meal access for this canteen
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
        [BsonRepresentation(BsonType.ObjectId)]
        public required string Id { get; set; } 
        public required string location { get; set; }
        public required string description { get; set; }
    }

}