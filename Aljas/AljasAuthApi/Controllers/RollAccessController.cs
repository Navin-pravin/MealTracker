using Microsoft.AspNetCore.Mvc;
using AljasAuthApi.Services;
using AljasAuthApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AljasAuthApi.Controllers
{
    [Route("api/role-access")]
    [ApiController]
    public class RoleAccessController : ControllerBase
    {
        private readonly RoleAccessService _roleAccessService;

        public RoleAccessController(RoleAccessService roleAccessService)
        {
            _roleAccessService = roleAccessService;
        }

        // ✅ Get all roles
        [HttpGet("summary")]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _roleAccessService.GetAllRolesAsync();
            return Ok(roles);
        }
   // ✅ Update Role Name & Allowed Modules
        [HttpPut("update/{roleId}")]
        public async Task<IActionResult> UpdateRole(string roleId, [FromBody] UpdateRoleRequest updateRequest)
        {
            bool success = await _roleAccessService.UpdateRoleAsync(roleId, updateRequest);
            if (!success)
                return NotFound(new { message = "Role not found." });

            return Ok(new { message = "Role updated successfully." });
        }

        // ✅ Create a new role (Auto-generates 3-digit RoleId)
        [HttpPost("create")]
        public async Task<IActionResult> CreateRole([FromBody] RoleAccess role)
        {
            await _roleAccessService.CreateRoleAsync(role);
            return Ok(new { message = "Role created successfully." });
        }
        [HttpGet("user-access/{Email}")]
public async Task<IActionResult> GetUserAccessByUsername(string Email)
{
    var userAccess = await _roleAccessService.GetUserAccessByUsernameAsync(Email);
    if (userAccess == null)
        return NotFound(new { message = "User or role not found." });

    return Ok(userAccess);
}

    }
}
