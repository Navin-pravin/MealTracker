using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

[BsonIgnoreExtraElements]
public class RawData
{
    [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();


    [BsonElement("created_date_utc")]
    [BsonRepresentation(BsonType.DateTime)]
    public DateTime CreatedDateUtc { get; set; }

    [BsonElement("meal_type")]
    public string MealType { get; set; }

    public string Device_uniqueid{get;set;}
}
