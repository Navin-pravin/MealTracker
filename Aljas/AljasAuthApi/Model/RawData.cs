using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace AljasAuthApi.Models
{
    [BsonIgnoreExtraElements]
    public class RawData
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("created_date_utc")]
        public DateTime CreatedDateUtc { get; set; }

        [BsonElement("sensor_attributes")]
        public SensorAttributes SensorAttributes { get; set; }

        [BsonElement("device_trigger")]
        public string DeviceTrigger { get; set; }

        [BsonElement("meal_type")]
        public string MealType { get; set; }

        [BsonElement("Type")]
        public string Type { get; set; } // Employee or SubContractor
    }

    [BsonIgnoreExtraElements]
    public class SensorAttributes
    {
        [BsonElement("person_id")]
        public string PersonId { get; set; }

        [BsonElement("card_number")]
        public string CardNumber { get; set; }
    }
    [BsonIgnoreExtraElements]
public class Rawdata
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

}