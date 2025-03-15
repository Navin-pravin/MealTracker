using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace ProjectHierarchyApi.Models
{
    public class CanteenConfiguration
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
        
        public string CanteenId { get; set; } = string.Empty;

        public List<MealTiming> MealTimings { get; set; } = new List<MealTiming>();
    }

    public class MealTiming
    {
        public string MealType { get; set; } = string.Empty;
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
    }
}
