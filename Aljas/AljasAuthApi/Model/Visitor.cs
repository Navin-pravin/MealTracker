using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace AljasAuthApi.Models
{
    public class Visitor
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
        public string IdNumber {get;set;}

        public required string VisitorName { get; set; }
        public required string Email { get; set; }
        public required DateTime StartDate { get; set; }
        public required DateTime EndDate { get; set; }
        public required string Role {get; set;}
        public required string VisitorCompany { get; set; }
        public required string ContactNo { get; set; }
        public required string Remarks {get;set;}
    }
}
