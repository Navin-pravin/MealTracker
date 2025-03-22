using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace AljasAuthApi.Models
{
    public class RoleAccess
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        public string RoleId { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;

        // ✅ Ensure AllowedModules is defined correctly
        [BsonElement("AllowedModules")]
        public AllowedModules AllowedModules { get; set; } = new AllowedModules();
    }

    public class AllowedModules
    {
        public bool Dashboard { get; set; } = false;
        public bool Events { get; set; } = false;

        [BsonElement("Process&Automation")]
        public bool ProcessAutomation { get; set; } = false;

        public bool Reports { get; set; } = false;
        public bool Administration { get; set; } = false;
    }
      public class UpdateRoleRequest
    { //public string RoleId { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public AllowedModules AllowedModules { get; set; } = new AllowedModules();
    }
}
