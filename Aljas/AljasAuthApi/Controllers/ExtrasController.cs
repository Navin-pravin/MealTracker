using Microsoft.AspNetCore.Mvc;
using AljasAuthApi.Models;
using AljasAuthApi.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AljasAuthApi.Controllers
{
    [Route("api/extras")]
    [ApiController]
    public class ExtrasController : ControllerBase
    {
        private readonly ExtrasService _extrasService;

        public ExtrasController(ExtrasService extrasService)
        {
            _extrasService = extrasService;
        }

        // ✅ Department APIs
        [HttpPost("add-department")]
        public async Task<IActionResult> AddDepartment([FromBody] Department department)
        {
            await _extrasService.AddDepartmentAsync(department);
            return Ok(new { message = "Department added successfully" });
        }

        [HttpPut("update-department/{id}")]
        public async Task<IActionResult> UpdateDepartment(string id, [FromBody] Department department)
        {
            var updated = await _extrasService.UpdateDepartmentAsync(id, department);
            if (!updated) return NotFound(new { message = "Department not found" });
            return Ok(new { message = "Department updated successfully" });
        }

        [HttpDelete("delete-department/{id}")]
        public async Task<IActionResult> DeleteDepartment(string id)
        {
            var deleted = await _extrasService.DeleteDepartmentAsync(id);
            if (!deleted) return NotFound(new { message = "Department not found" });
            return Ok(new { message = "Department deleted successfully" });
        }

        [HttpGet("department-summary")]
        public async Task<IActionResult> GetDepartmentSummary()
        {
            var departments = await _extrasService.GetDepartmentSummaryAsync();
            return Ok(departments);
        }

        // ✅ Company APIs
        [HttpPost("add-company")]
        public async Task<IActionResult> AddCompany([FromBody] Company company)
        {
            await _extrasService.AddCompanyAsync(company);
            return Ok(new { message = "Company added successfully" });
        }

        [HttpPut("update-company/{id}")]
        public async Task<IActionResult> UpdateCompany(string id, [FromBody] Company company)
        {
            var updated = await _extrasService.UpdateCompanyAsync(id, company);
            if (!updated) return NotFound(new { message = "Company not found" });
            return Ok(new { message = "Company updated successfully" });
        }

        [HttpDelete("delete-company/{id}")]
        public async Task<IActionResult> DeleteCompany(string id)
        {
            var deleted = await _extrasService.DeleteCompanyAsync(id);
            if (!deleted) return NotFound(new { message = "Company not found" });
            return Ok(new { message = "Company deleted successfully" });
        }

        [HttpGet("company-summary")]
        public async Task<IActionResult> GetCompanySummary()
        {
            var companies = await _extrasService.GetCompanySummaryAsync();
            return Ok(companies);
        }

        // ✅ Role APIs with Canteen and Meal Access
        [HttpPost("add-role")]
        public async Task<IActionResult> AddRole([FromBody] Role role)
        {
            await _extrasService.AddRoleAsync(role);
            return Ok(new { message = "Role added successfully" });
        }

        [HttpPut("update-role/{id}")]
        public async Task<IActionResult> UpdateRole(string id, [FromBody] Role role)
        {
            var updated = await _extrasService.UpdateRoleAsync(id, role);
            if (!updated) return NotFound(new { message = "Role not found" });
            return Ok(new { message = "Role updated successfully" });
        }

        [HttpDelete("delete-role/{id}")]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var deleted = await _extrasService.DeleteRoleAsync(id);
            if (!deleted) return NotFound(new { message = "Role not found" });
            return Ok(new { message = "Role deleted successfully" });
        }

        [HttpGet("role-summary")]
        public async Task<IActionResult> GetRoleSummary()
        {
            var roles = await _extrasService.GetRoleSummaryAsync();
            return Ok(roles);
        }

        // ✅ Designation APIs
        [HttpPost("add-designation")]
        public async Task<IActionResult> AddDesignation([FromBody] Designation designation)
        {
            await _extrasService.AddDesignationAsync(designation);
            return Ok(new { message = "Designation added successfully" });
        }

        [HttpPut("update-designation/{id}")]
        public async Task<IActionResult> UpdateDesignation(string id, [FromBody] Designation designation)
        {
            var updated = await _extrasService.UpdateDesignationAsync(id, designation);
            if (!updated) return NotFound(new { message = "Designation not found" });
            return Ok(new { message = "Designation updated successfully" });
        }

        [HttpDelete("delete-designation/{id}")]
        public async Task<IActionResult> DeleteDesignation(string id)
        {
            var deleted = await _extrasService.DeleteDesignationAsync(id);
            if (!deleted) return NotFound(new { message = "Designation not found" });
            return Ok(new { message = "Designation deleted successfully" });
        }

        [HttpGet("designation-summary")]
        public async Task<IActionResult> GetDesignationSummary()
        {
            var designations = await _extrasService.GetDesignationSummaryAsync();
            return Ok(designations);
        }
    }
}
