using AljasAuthApi.Config;
using AljasAuthApi.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AljasAuthApi.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> _users;

        public UserService(MongoDbSettings dbSettings)
        {
            var client = new MongoClient(dbSettings.ConnectionString);
            var database = client.GetDatabase(dbSettings.DatabaseName);
            _users = database.GetCollection<User>("Users");
        }

        // ✅ Get User Summary
        public async Task<List<UserSummary>> GetUserSummaryAsync()
        {
            return await _users.Find(_ => true)
                .Project(u => new UserSummary
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    ContactNo = u.ContactNo,
                    RoleName = u.RoleName,
                    RoleAccess = u.AllowedModules
                })
                .ToListAsync();
        }

        // ✅ Create User (Without Role Sync)
        public async Task CreateUserAsync(CreateUserRequest request)
        {
            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                ContactNo = request.ContactNo,
                Password = request.Password,
                RoleName = request.RoleName,
                AllowedModules = request.AllowedModules // ✅ No Role Sync, Uses Request Data
            };

            await _users.InsertOneAsync(user);
        }

        // ✅ Update User (No Role Sync)
        public async Task<bool> UpdateUserAsync(UpdateUserRequest request)
        {
            var update = Builders<User>.Update
                .Set(u => u.Username, request.Username)
                .Set(u => u.Email, request.Email)
                .Set(u => u.ContactNo, request.ContactNo)
                .Set(u => u.RoleName, request.RoleName)
                .Set(u => u.AllowedModules, request.AllowedModules); // ✅ Directly Updates AllowedModules

            if (!string.IsNullOrEmpty(request.Password))
            {
                update = update.Set(u => u.Password, request.Password);
            }

            var result = await _users.UpdateOneAsync(u => u.Id == request.Id, update);
            return result.ModifiedCount > 0;
        }

        // ✅ Delete User
        public async Task<bool> DeleteUserAsync(string id)
        {
            var result = await _users.DeleteOneAsync(u => u.Id == id);
            return result.DeletedCount > 0;
        }
    }
}
