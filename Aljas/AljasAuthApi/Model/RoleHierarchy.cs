using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace AljasAuthApi.Models
{
    public class Role1
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        [BsonElement("RoleId")]
        public string RoleId { get; set; } = string.Empty;

        [BsonElement("RoleName")]
        public string RoleName { get; set; } = string.Empty;

        [BsonElement("Description")]
        public string Description { get; set; } = string.Empty;
    }

    public class RoleHierarchy
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        [BsonElement("RoleId")]
        public string RoleId { get; set; } = string.Empty;

        [BsonElement("RoleName")]
        public string RoleName { get; set; } = string.Empty;

        [BsonElement("Description")]
        public string Description { get; set; } = string.Empty;

        [BsonElement("Locations")]
        public List<Location1> Locations { get; set; } = new List<Location1>();
    }

    public class Location1
    {
        public string LocationId { get; set; } = string.Empty;
        public string LocationName { get; set; } = string.Empty;
        public List<Canteen1> Canteens { get; set; } = new List<Canteen1>();
    }

    public class Canteen1
    {
        public string CanteenId { get; set; } = string.Empty;
        public string CanteenName { get; set; } = string.Empty;
        public List<MealConfiguration1> MealConfigurations { get; set; } = new List<MealConfiguration1>();
    }

    public class MealConfiguration1
    {
        public string MealType { get; set; } = string.Empty;
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}