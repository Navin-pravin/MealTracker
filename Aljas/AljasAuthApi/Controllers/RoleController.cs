using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using AljasAuthApi.Models;
using AljasAuthApi.Services;

namespace AljasAuthApi.Controllers
{
    [Route("api/role")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly RoleService _roleService;

        public RoleController(RoleService roleService)
        {
            _roleService = roleService;
        }

        // ✅ Create Role (Auto-generates Role ID)
        [HttpPost("create")]
        public async Task<IActionResult> CreateRole([FromBody] Role role)
        {
            if (role == null || string.IsNullOrEmpty(role.RoleName))
                return BadRequest(new { message = "Invalid request. Role name is required." });

            bool created = await _roleService.CreateRoleAsync(role);
            if (!created)
                return BadRequest(new { message = "Failed to create role." });

            return Ok(new { message = "Role created successfully", role });
        }

        // ✅ Get All Roles (Summary)
        [HttpGet("summary")]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _roleService.GetRolesAsync();
            return Ok(roles);
        }

        // ✅ Update Role (Role ID remains unchanged)
        [HttpPut("update")]
        public async Task<IActionResult> UpdateRole([FromQuery] string Id, [FromBody] Role updatedRole)
        {
            if (string.IsNullOrEmpty(Id) || string.IsNullOrEmpty(updatedRole.RoleName))
                return BadRequest(new { message = "Role ID and new Role Name are required." });

            bool updated = await _roleService.UpdateRoleAsync(Id, updatedRole.RoleName);
            if (!updated)
                return NotFound(new { message = "Role not found." });

            return Ok(new { message = "Role updated successfully." });
        }

        // ✅ Delete Role
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteRole([FromQuery] string Id)
        {
            if (string.IsNullOrEmpty(Id))
                return BadRequest(new { message = "Role ID is required." });

            bool deleted = await _roleService.DeleteRoleAsync(Id);
            if (!deleted)
                return NotFound(new { message = "Role not found." });

            return Ok(new { message = "Role deleted successfully." });
        }
    }
}
