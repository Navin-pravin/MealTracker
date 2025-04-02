using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using AljasAuthApi.Models;
using AljasAuthApi.Services;

namespace AljasAuthApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleHierarchyController : ControllerBase
    {
        private readonly RoleHierarchyService _roleHierarchyService;

        public RoleHierarchyController(RoleHierarchyService roleHierarchyService)
        {
            _roleHierarchyService = roleHierarchyService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateRole([FromBody] RoleHierarchy roleHierarchy)
        {
            var result = await _roleHierarchyService.CreateRoleAsync(roleHierarchy);
            return result ? Ok(new { message = "Role created successfully" }) : BadRequest("Failed to create role");
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await _roleHierarchyService.GetAllRolesAsync();
            return Ok(roles);
        }
          [HttpGet("get/{roleId}")]
        public async Task<IActionResult> GetRoleById(string roleId)
        {
            var role = await _roleHierarchyService.GetRoleByIdAsync(roleId);
            return role != null ? Ok(role) : NotFound(new { message = "Role not found" });
        }

        [HttpPut("update/{roleId}")]
        public async Task<IActionResult> UpdateRole(string roleId, [FromBody] RoleHierarchy updatedRole)
        {
            var result = await _roleHierarchyService.UpdateRoleAsync(roleId, updatedRole);
            return result ? Ok(new { message = "Role updated successfully" }) : NotFound("Role not found");
        }

        [HttpDelete("delete/{roleId}")]
        public async Task<IActionResult> DeleteRole(string roleId)
        {
            var result = await _roleHierarchyService.DeleteRoleAsync(roleId);
            return result ? Ok(new { message = "Role deleted successfully" }) : NotFound("Role not found");
        }
    }
}