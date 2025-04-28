// Models/Coupon.cs
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace ProjectHierarchyApi.Models
{
    public class Coupon
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        public string CouponCode { get; set; } = string.Empty;
       //public string SerialCode { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string SerialCode { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
        public bool Status { get; set; } = true;
        
        public DateTime Createdat {get;set;}
        public string Createdby {get;set;}
        public DateTime Modifiedat {get;set;}
        public string Redeemedcanteen {get;set;}
        
        public string Modifiedby {get;set;}
        public bool Redeemstatus {get;set;}
        public string Assignedto {get;set;}
                public string AssignedEmployee {get;set;}


        public DateTime Assignedat {get;set;}

        public DateTime Redeemedat {get;set;}
        
        
        
        
        

    }
}
