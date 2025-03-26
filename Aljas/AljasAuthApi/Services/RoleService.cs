using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using AljasAuthApi.Models;

namespace AljasAuthApi.Services
{
    public class RoleService
    {
        private readonly IMongoCollection<Role> _roles;

        public RoleService(MongoDbSettings dbSettings)
        {
            var client = new MongoClient(dbSettings.ConnectionString);
            var database = client.GetDatabase(dbSettings.DatabaseName);
            _roles = database.GetCollection<Role>("Role1");
        }

        // ✅ Generate a Unique 3-digit Role ID
        private async Task<string> GenerateRoleIdAsync()
        {
            int newRoleId;
            bool exists;
            var random = new Random();

            do
            {
                newRoleId = random.Next(100, 1000); // Generates a number between 100-999
                exists = await _roles.Find(r => r.RoleId == newRoleId.ToString()).AnyAsync();
            } while (exists);

            return newRoleId.ToString();
        }

        // ✅ Create Role with Auto-generated 3-digit Role ID
        public async Task<bool> CreateRoleAsync(Role role)
        {
            role.RoleId = await GenerateRoleIdAsync(); // Assigning auto-generated Role ID
            await _roles.InsertOneAsync(role);
            return true;
        }

        // ✅ Get All Roles (Summary)
        public async Task<List<Role>> GetRolesAsync()
        {
            return await _roles.Find(_ => true).ToListAsync();
        }

        // ✅ Update Role (Role ID remains unchanged)
        public async Task<bool> UpdateRoleAsync(string Id, string newRoleName)
        {
            var update = Builders<Role>.Update.Set(r => r.RoleName, newRoleName);
            var result = await _roles.UpdateOneAsync(r => r.Id == Id, update);
            return result.ModifiedCount > 0;
        }

        // ✅ Delete Role
        public async Task<bool> DeleteRoleAsync(string Id)
        {
            var result = await _roles.DeleteOneAsync(r => r.Id == Id);
            return result.DeletedCount > 0;
        }
    }
}
