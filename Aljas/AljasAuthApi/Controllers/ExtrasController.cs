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

        // Department Endpoints
        [HttpPost("add-department")]
        public async Task<IActionResult> AddDepartment([FromBody] Department department)
        {
            if (department == null || string.IsNullOrEmpty(department.DepartmentName))
                return BadRequest(new { message = "Invalid department data" });

            await _extrasService.AddDepartmentAsync(department);
            return Ok(new { message = "Department added successfully" });
        }

        [HttpPut("update-department/{id}")]
        public async Task<IActionResult> UpdateDepartment(string id, [FromBody] Department department)
        {
            if (string.IsNullOrEmpty(id) || department == null)
                return BadRequest(new { message = "Invalid request" });

            var updated = await _extrasService.UpdateDepartmentAsync(id, department);
            if (!updated) return NotFound(new { message = "Department not found" });

            return Ok(new { message = "Department updated successfully" });
        }

        [HttpDelete("delete-department/{id}")]
        public async Task<IActionResult> DeleteDepartment(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest(new { message = "Invalid ID" });

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

        // Company Endpoints
        [HttpPost("add-company")]
        public async Task<IActionResult> AddCompany([FromBody] Company company)
        {
            if (company == null || string.IsNullOrEmpty(company.CompanyName))
                return BadRequest(new { message = "Invalid company data" });

            await _extrasService.AddCompanyAsync(company);
            return Ok(new { message = "Company added successfully" });
        }

        [HttpPut("update-company/{id}")]
        public async Task<IActionResult> UpdateCompany(string id, [FromBody] Company company)
        {
            if (string.IsNullOrEmpty(id) || company == null)
                return BadRequest(new { message = "Invalid request" });

            var updated = await _extrasService.UpdateCompanyAsync(id, company);
            if (!updated) return NotFound(new { message = "Company not found" });

            return Ok(new { message = "Company updated successfully" });
        }

        [HttpDelete("delete-company/{id}")]
        public async Task<IActionResult> DeleteCompany(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest(new { message = "Invalid ID" });

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

        // Designation Endpoints
        [HttpPost("add-designation")]
        public async Task<IActionResult> AddDesignation([FromBody] Designation designation)
        {
            if (designation == null || string.IsNullOrEmpty(designation.DesignationTitle))
                return BadRequest(new { message = "Invalid designation data" });

            await _extrasService.AddDesignationAsync(designation);
            return Ok(new { message = "Designation added successfully" });
        }

        [HttpPut("update-designation/{id}")]
        public async Task<IActionResult> UpdateDesignation(string id, [FromBody] Designation designation)
        {
            if (string.IsNullOrEmpty(id) || designation == null)
                return BadRequest(new { message = "Invalid request" });

            var updated = await _extrasService.UpdateDesignationAsync(id, designation);
            if (!updated) return NotFound(new { message = "Designation not found" });

            return Ok(new { message = "Designation updated successfully" });
        }

        [HttpDelete("delete-designation/{id}")]
        public async Task<IActionResult> DeleteDesignation(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest(new { message = "Invalid ID" });

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

        // Location Endpoints
        [HttpPost("add-location")]
        public async Task<IActionResult> AddLocation([FromBody] CLocation location)
        {
            if (location == null || string.IsNullOrEmpty(location.location))
                return BadRequest(new { message = "Invalid location data" });

            await _extrasService.AddLocationAsync(location);
            return Ok(new { message = "Location added successfully" });
        }

        [HttpPut("update-location/{id}")]
        public async Task<IActionResult> UpdateLocation(string id, [FromBody] CLocation location)
        {
            if (string.IsNullOrEmpty(id) || location == null)
                return BadRequest(new { message = "Invalid request" });

            var updated = await _extrasService.UpdateLocationAsync(id, location);
            if (!updated) return NotFound(new { message = "Location not found" });

            return Ok(new { message = "Location updated successfully" });
        }

        [HttpDelete("delete-location/{id}")]
        public async Task<IActionResult> DeleteLocation(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest(new { message = "Invalid ID" });

            var deleted = await _extrasService.DeleteLocationAsync(id);
            if (!deleted) return NotFound(new { message = "Location not found" });

            return Ok(new { message = "Location deleted successfully" });
        }

        [HttpGet("location-summary")]
        public async Task<IActionResult> GetLocationSummary()
        {
            var locations = await _extrasService.GetLocationSummaryAsync();
            return Ok(locations);
        }

        [HttpPost("add-meal-config")]
public async Task<IActionResult> AddMealConfiguration([FromBody] MealConfiguration config)
{
    if (config == null || string.IsNullOrEmpty(config.MealType))
        return BadRequest(new { message = "Meal configuration data is invalid" });

    // Check if StartTime and EndTime are valid if provided
    if (!string.IsNullOrEmpty(config.StartTime?.ToString()) && 
        !TimeSpan.TryParse(config.StartTime.ToString(), out _))
    {
        return BadRequest(new { message = "Invalid StartTime format. Use HH:mm:ss" });
    }

    if (!string.IsNullOrEmpty(config.EndTime?.ToString()) && 
        !TimeSpan.TryParse(config.EndTime.ToString(), out _))
    {
        return BadRequest(new { message = "Invalid EndTime format. Use HH:mm:ss" });
    }

    await _extrasService.AddMealConfigurationAsync(config);
    return Ok(new { message = "Meal configuration added successfully" });
}
        [HttpPut("update-meal-config/{id}")]
        public async Task<IActionResult> UpdateMealConfiguration(string id, [FromBody] MealConfiguration config)
        {
            if (string.IsNullOrEmpty(id) || config == null)
                return BadRequest(new { message = "Invalid request" });

            var updated = await _extrasService.UpdateMealConfigurationAsync(id, config);
            if (!updated) return NotFound(new { message = "Meal configuration not found" });

            return Ok(new { message = "Meal configuration updated successfully" });
        }

        [HttpDelete("delete-meal-config/{id}")]
        public async Task<IActionResult> DeleteMealConfiguration(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest(new { message = "Invalid ID" });

            var deleted = await _extrasService.DeleteMealConfigurationAsync(id);
            if (!deleted) return NotFound(new { message = "Meal configuration not found" });

            return Ok(new { message = "Meal configuration deleted successfully" });
        }

        [HttpGet("meal-config-summary")]
        public async Task<IActionResult> GetMealConfigurations()
        {
            var configs = await _extrasService.GetMealConfigurationsAsync();
            return Ok(configs);
        }

        [HttpGet("active-meal-configs")]
        public async Task<IActionResult> GetActiveMealConfigurations()
        {
            var activeConfigs = await _extrasService.GetActiveMealConfigurationsAsync();
            return Ok(activeConfigs);
        }
    }
}