using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace AljasAuthApi.Models
{
    public class ReportRequest
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Type { get; set; } 
        public List<string>? CanteenIds { get; set; }
        public List<string>? MealTypes { get; set; }

        
    }
     public class ReportModel
    {
        [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } 
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public required string Type { get; set; }
        public DateTime CreatedOn { get; set; }
        public int Count { get; set; }

        // New fields
        public string? CanteenName { get; set; }
        public string? MealType { get; set; }
    }
    
}