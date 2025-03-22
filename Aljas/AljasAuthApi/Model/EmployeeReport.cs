using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace AljasAuthApi.Models
{
    public class EmployeeReport
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public DateTime Date { get; set; }
        public string IdNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public string Designation { get; set; }
        public string RoleName { get; set; }
        public string Location { get; set; }
        public string Department { get; set; }
        public string CarBadgeNumber { get; set; }
        public string DeviceName { get; set; }
        public string PunchingTime { get; set; }
        public string Canteen { get; set; }
        public string MealType { get; set; }
    }
}
