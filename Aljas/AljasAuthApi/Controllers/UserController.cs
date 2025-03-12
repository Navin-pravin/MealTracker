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

        [HttpPost("create")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            await _userService.CreateUserAsync(request);
            return Ok(new { message = "User created successfully" });
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetUserSummary()
        {
            var users = await _userService.GetUserSummaryAsync();
            return Ok(users);
        }

        // ✅ Update User API
        [HttpPut("update")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest request)
        {
            if (string.IsNullOrEmpty(request.Id))
                return BadRequest(new { message = "User ID is required." });

            bool updated = await _userService.UpdateUserAsync(request);
            if (!updated)
                return NotFound(new { message = "User not found or no changes made." });

            return Ok(new { message = "User updated successfully." });
        }

        // ✅ Delete User API
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            bool deleted = await _userService.DeleteUserAsync(id);
            if (!deleted)
                return NotFound(new { message = "User not found." });

            return Ok(new { message = "User deleted successfully." });
        }
    }
}
