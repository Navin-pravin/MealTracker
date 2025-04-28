using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AljasAuthApi.Models;

namespace AljasAuthApi.Services
{
    public class RoleHierarchyService
    {
        private readonly IMongoCollection<RoleHierarchy> _roleHierarchy;
        private readonly IMongoCollection<Role> _roles;
        //public readonly EmployeeService _employeeservice;

        public RoleHierarchyService(MongoDbSettings dbSettings)
        {
            var client = new MongoClient(dbSettings.ConnectionString);
            var database = client.GetDatabase(dbSettings.DatabaseName);
            _roleHierarchy = database.GetCollection<RoleHierarchy>("RoleHierarchy");
            _roles = database.GetCollection<Role>("Role1");
            //_employeeservice=employeeService;
        }

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

        public async Task<bool> CreateRoleAsync(RoleHierarchy roleHierarchy)
        {
            roleHierarchy.RoleId = await GenerateRoleIdAsync(); 

            var role = new Role
            {
                RoleId = roleHierarchy.RoleId,
                RoleName = roleHierarchy.RoleName,
                Description = roleHierarchy.Description
            };

            await _roles.InsertOneAsync(role);
            await _roleHierarchy.InsertOneAsync(roleHierarchy);

            return true;
        }

        public async Task<List<RoleHierarchy>> GetAllRolesAsync()
        {
            return await _roleHierarchy.Find(_ => true).ToListAsync();
        }

        public async Task<RoleHierarchy> GetRoleByIdAsync(string roleId)
        {
            return await _roleHierarchy.Find(r => r.RoleId == roleId).FirstOrDefaultAsync();
        }


        public async Task<bool> UpdateRoleAsync(string roleId, RoleHierarchy updatedRole)
        {
            var updateResult = await _roleHierarchy.ReplaceOneAsync(r => r.RoleId == roleId, updatedRole);
            if (updateResult.ModifiedCount > 0)
            {
                var roleUpdate = Builders<Role>.Update
                    .Set(r => r.RoleName, updatedRole.RoleName)
                    .Set(r => r.Description, updatedRole.Description);

                await _roles.UpdateOneAsync(r => r.RoleId == roleId, roleUpdate);
                return true;
            }
            return false;
        }
public async Task<List<string>> GetRoleNamesByCanteenIdAsync(string canteenId)
{
    var filter = Builders<RoleHierarchy>.Filter.ElemMatch(
        r => r.Locations, loc =>
            loc.Canteens.Any(c => c.CanteenId == canteenId)
    );

    var roles = await _roleHierarchy.Find(_ => true).ToListAsync();

    var matchedRoles = roles.Where(role =>
        role.Locations.Any(loc =>
            loc.Canteens.Any(c => c.CanteenId == canteenId)
        )
    ).Select(r => r.RoleName).Distinct().ToList();

    return matchedRoles;
}

        public async Task<bool> DeleteRoleAsync(string roleId)
        {
            var roleHierarchyResult = await _roleHierarchy.DeleteOneAsync(r => r.RoleId == roleId);
            var roleResult = await _roles.DeleteOneAsync(r => r.RoleId == roleId);
            return roleHierarchyResult.DeletedCount > 0 && roleResult.DeletedCount > 0;
        }
    }
}