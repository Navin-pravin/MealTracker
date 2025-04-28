using AljasAuthApi.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AljasAuthApi.Services
{
    public class RoleAccessService
    {
        private readonly IMongoCollection<RoleAccess> _roles;
        private readonly IMongoCollection<User> _users;

        public RoleAccessService(IMongoDatabase database)
        {
            _roles = database.GetCollection<RoleAccess>("RoleAccess");
            _users = database.GetCollection<User>("Users");
        }

        // ✅ Get all roles
        public async Task<List<RoleAccess>> GetAllRolesAsync() =>
            await _roles.Find(_ => true).ToListAsync();

        // ✅ Get role by RoleId
        public async Task<RoleAccess?> GetRoleByIdAsync(string roleId) =>
            await _roles.Find(r => r.RoleId == roleId).FirstOrDefaultAsync();

        // ✅ Update role modules based on RoleId
    public async Task<bool> UpdateRoleAsync(string roleId, UpdateRoleRequest updateRequest)
        {
            var update = Builders<RoleAccess>.Update
                .Set(r => r.RoleName, updateRequest.RoleName)
                .Set(r => r.AllowedModules, updateRequest.AllowedModules);

           var result = await _roles.UpdateOneAsync(r => r.RoleId == roleId, update);

            return result.ModifiedCount > 0;
        }


        // ✅ Generate a Unique 3-digit RoleId
        private readonly Random _random = new();
        private async Task<string> GenerateUniqueRoleIdAsync()
        {
            while (true)
            {
                string randomRoleId = _random.Next(100, 1000).ToString(); // Generates 3-digit number

                var existingRole = await _roles.Find(r => r.RoleId == randomRoleId).FirstOrDefaultAsync();
                if (existingRole == null)
                {
                    return randomRoleId; // ✅ Unique RoleId
                }
            }
        }

        // ✅ Create a new role with a Unique RoleId
        public async Task CreateRoleAsync(RoleAccess role)
        {
            role.RoleId = await GenerateUniqueRoleIdAsync(); // Assign unique 3-digit RoleId
            await _roles.InsertOneAsync(role);
        }
        public async Task<UserAccessResponse?> GetUserAccessByUsernameAsync(string Email)
{
    var user = await _users.Find(u => u.Email == Email).FirstOrDefaultAsync();
    if (user == null) return null;

    var roleAccess = await _roles.Find(r => r.RoleName == user.RoleName).FirstOrDefaultAsync();
    if (roleAccess == null) return null;

    return new UserAccessResponse
    {
        Id = user.Id,
        Username = user.Email,
        RoleName = user.RoleName,
        RoleId=roleAccess.RoleId,
        AllowedModules = roleAccess.AllowedModules
    };
}

    }
}
