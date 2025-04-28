using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AljasAuthApi.Models
{
    public class SubContractor
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        // Correct BsonElement mappings to match the MongoDB collection fields
        [BsonElement("contractorname")]
        public string ContractorName { get; set; } = string.Empty;

        [BsonElement("contractor_id")]
        public string ContractorId { get; set; } = string.Empty;

        [BsonElement("companyname")]
        public string CompanyName { get; set; } = string.Empty;

        [BsonElement("project_name")]
        public string ProjectName { get; set; } = string.Empty;

        [BsonElement("address")]
        public string Address { get; set; } = string.Empty;

        [BsonElement("phone_no")]
        public string PhoneNo { get; set; } = string.Empty;

        [BsonElement("nationality")]
        public string Nationality { get; set; } = string.Empty;

        [BsonElement("vehicle_name")]
        public string VehicleName { get; set; } = string.Empty;

        [BsonElement("vehicle_id")]
        public string VehicleId { get; set; } = string.Empty;

        [BsonElement("contractor_image")]
        public string ImageUrl { get; set; } = string.Empty;

        [BsonElement("role")]
        public string Role { get; set; } = string.Empty;
    }
}