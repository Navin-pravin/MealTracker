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
        private readonly IMongoCollection<RoleAccess> _roleAccess;

        public UserService(MongoDbSettings dbSettings)
        {
            var client = new MongoClient(dbSettings.ConnectionString);
            var database = client.GetDatabase(dbSettings.DatabaseName);
            _users = database.GetCollection<User>("Users");
            _roleAccess = database.GetCollection<RoleAccess>("RoleAccess"); // ✅ Ensure RoleAccess collection exists
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
                    RoleName = u.RoleName
                })
                .ToListAsync();
        }

        // ✅ Get Allowed Modules by User ID (Role-Based)
      /*  public async Task<Dictionary<string, bool>> GetAllowedModulesByUserIdAsync(string userId)
        {
            var user = await _users.Find(u => u.Id == userId).FirstOrDefaultAsync();
            if (user == null)
                return new Dictionary<string, bool>(); // ❌ User not found → return empty dictionary

            var roleAccess = await _roleAccess.Find(r => r.RoleName == user.RoleName).FirstOrDefaultAsync();
            return roleAccess?.AllowedModules ?? new Dictionary<string, bool>(); // ✅ Return modules or empty dictionary
        }
*/
        // ✅ Create User (Ensure Required Fields)
        public async Task<bool> CreateUserAsync(CreateUserRequest request)
{
    // ✅ Check if Role exists in RoleAccess collection
    var roleExists = await _roleAccess.Find(r => r.RoleName == request.RoleName).AnyAsync();
    if (!roleExists)
    {
        return false; // ❌ Role does not exist
    }

    // ✅ Check if password and confirm password match
    if (request.Password != request.ConfirmPassword)
    {
        return false; // ❌ Passwords do not match
    }

    var user = new User
    {
        Username = request.Username,
        Email = request.Email,
        ContactNo = request.ContactNo,
        Password = request.Password,
        ConfirmPassword = request.ConfirmPassword,
        RoleName = request.RoleName,
        CreatedAt = DateTime.UtcNow
    };

    await _users.InsertOneAsync(user);
    return true;
}


        // ✅ Update User (Ensure Role Exists Before Updating)
       public async Task<bool> UpdateUserAsync(UpdateUserRequest request)
{
    // ✅ Check if Role exists
    var roleExists = await _roleAccess.Find(r => r.RoleName == request.RoleName).AnyAsync();
    if (!roleExists)
    {
        return false; // ❌ Role does not exist
    }

    // ✅ Ensure Password and ConfirmPassword are not null and match
    if (string.IsNullOrEmpty(request.Password) || string.IsNullOrEmpty(request.ConfirmPassword) || request.Password != request.ConfirmPassword)
    {
        return false; // ❌ Password and ConfirmPassword must match
    }

    var update = Builders<User>.Update
        .Set(u => u.Username, request.Username)
        .Set(u => u.Email, request.Email)
        .Set(u => u.ContactNo, request.ContactNo)
        .Set(u => u.RoleName, request.RoleName)
        .Set(u => u.Password, request.Password)
        .Set(u => u.ConfirmPassword, request.ConfirmPassword)
        .Set(u => u.CreatedAt, DateTime.UtcNow);

    var result = await _users.UpdateOneAsync(u => u.Id == request.Id, update);
    return result.ModifiedCount > 0;
}


        // ✅ Delete User (Prevent Super Admin Deletion)
        public async Task<bool> DeleteUserAsync(string id)
        {
            if (id == "67de4a5cc3c6624bdee43269") // Prevent Super Admin deletion
            {
                return false;
            }

            var result = await _users.DeleteOneAsync(u => u.Id == id);
            return result.DeletedCount > 0;
        }

        // ✅ Get User by ID
        public async Task<User?> GetUserByIdAsync(string id)
        {
            return await _users.Find(u => u.Id == id).FirstOrDefaultAsync();
        }
        public async Task<bool> ChangePasswordAsync(ChangePasswordRequest request)
{
    var user = await _users.Find(u => u.Id == request.UserId).FirstOrDefaultAsync();
    if (user == null || user.Password != request.CurrentPassword)
    {
        return false; // ❌ User not found or current password is incorrect
    }

    if (request.NewPassword != request.ConfirmNewPassword)
    {
        return false; // ❌ New password and confirm password do not match
    }

    var update = Builders<User>.Update
        .Set(u => u.Password, request.NewPassword)
        .Set(u => u.ConfirmPassword, request.ConfirmNewPassword);

    var result = await _users.UpdateOneAsync(u => u.Id == request.UserId, update);
    return result.ModifiedCount > 0;
}

    }
}
