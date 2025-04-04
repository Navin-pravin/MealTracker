using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AljasAuthApi.Models
{
    public class Employee
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public required string Id { get; set; } = ObjectId.GenerateNewId().ToString();
        public required string IDNumber { get; set; }
        public required string Firstname { get; set; }
        public required string Lastname { get; set; }
        public required string Email{get;set;}
        public required String StartDate{get; set;}


        public required string EndDate {get; set; }

        public required string Phone_no { get; set; }
        public required string Dept { get; set; } 
        public required string Role { get; set; }
        public required string designation{ get; set; }
        public required string imageUrl { get; set; }
        public required string Company { get; set; }
         public required string location {get; set; }
        public required string Referenceid { get; set; }
        public required string CardBadgeNumber { get; set; }
        [BsonElement("status")]
        public required bool Status { get; set; } = true;



        

    }
}
