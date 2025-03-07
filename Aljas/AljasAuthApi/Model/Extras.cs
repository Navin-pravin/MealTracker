using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace AljasAuthApi.Models
{
    public class Department
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public required string DepartmentName { get; set; }
    }

    public class Company
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public required string CompanyName { get; set; }
    }

    public class Role
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public required string RoleTitle { get; set; }
        public required List<string> CanteenAccess { get; set; } = new();
        public required List<string> MealAccess { get; set; } = new();
    }

    public class Designation
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public required string DesignationTitle { get; set; }
    }
}
