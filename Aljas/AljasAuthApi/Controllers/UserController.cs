using Microsoft.AspNetCore.Mvc;
using AljasAuthApi.Services;
using AljasAuthApi.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace AljasAuthApi.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        // ✅ Create User API (Ensures Role Validation)
        [HttpPost("create")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.RoleName))
                return BadRequest(new { message = "Invalid request. Role name is required." });

            bool created = await _userService.CreateUserAsync(request);

            if (!created)
                return BadRequest(new { message = "Failed to create user. Role does not exist." });

            return Ok(new { message = "User created successfully" });
        }

        // ✅ Get User Summary API
        [HttpGet("summary")]
        public async Task<IActionResult> GetUserSummary()
        {
            var users = await _userService.GetUserSummaryAsync();
            return Ok(users);
        }

        // ✅ Update User API (Ensures Role Validation) with Required Query Parameter
        [HttpPut("update")]
        public async Task<IActionResult> UpdateUser([FromQuery] string id, [FromBody] UpdateUserRequest request)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest(new { message = "User ID is required." });

            request.Id = id; // Ensure request object has the correct ID

            bool updated = await _userService.UpdateUserAsync(request);
            if (!updated)
                return NotFound(new { message = "User not found or role does not exist." });

            return Ok(new { message = "User updated successfully." });
        }

        // ✅ Delete User API (Super Admin Protection) with Required Query Parameter
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteUser([FromQuery] string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest(new { message = "User ID is required." });

            if (id == "67d7c0c81747577259e4326b")
                return BadRequest(new { message = "Super Admin cannot be deleted." });

            bool deleted = await _userService.DeleteUserAsync(id);
            if (!deleted)
                return NotFound(new { message = "User not found." });

            return Ok(new { message = "User deleted successfully." });
        }

        // ✅ Fetch Allowed Modules for a User by Role
       /* [HttpGet("allowed-modules/{userId}")]
        public async Task<IActionResult> GetAllowedModules(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return BadRequest(new { message = "User ID is required." });

            var modules = await _userService.GetAllowedModulesByUserIdAsync(userId);

            if (modules == null || modules.Count == 0)
                return NotFound(new { message = "No allowed modules found for the user's role." });

            return Ok(new { userId, allowedModules = modules });
        }*/

        // ✅ Get User By ID API
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest(new { message = "User ID is required." });

            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
                return NotFound(new { message = "User not found." });

            return Ok(user);
        }
    }
}
