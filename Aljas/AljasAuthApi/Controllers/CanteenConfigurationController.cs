using Microsoft.AspNetCore.Mvc;
using ProjectHierarchyApi.Models;
using ProjectHierarchyApi.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectHierarchyApi.Controllers
{
    [Route("api/canteen-configurations")]
    [ApiController]
    public class CanteenConfigurationController : ControllerBase
    {
        private readonly CanteenConfigurationService _configService;
        private readonly CanteenService _canteenService;

        public CanteenConfigurationController(CanteenConfigurationService configService, CanteenService canteenService)
        {
            _configService = configService;
            _canteenService = canteenService;
        }

        // ✅ Get meal timings for a selected canteen
        [HttpGet("{canteenId}")]
        public async Task<ActionResult<CanteenConfiguration>> GetConfiguration(string canteenId)
        {
            var config = await _configService.GetConfigurationByCanteenIdAsync(canteenId);
            if (config == null)
                return NotFound(new { message = "Canteen configuration not found" });

            return Ok(config);
        }

        // ✅ Create/Update meal timings for a selected canteen
        [HttpPost("upsert")]
        public async Task<IActionResult> UpsertConfiguration([FromBody] CanteenConfiguration config)
        {
            if (config == null || string.IsNullOrEmpty(config.CanteenId))
                return BadRequest(new { message = "Invalid configuration data" });

            // ✅ Validate if the canteen exists
            var canteen = await _canteenService.GetCanteenByIdAsync(config.CanteenId);
            if (canteen == null)
                return NotFound(new { message = "Canteen not found. Please select a valid canteen." });

            bool success = await _configService.UpsertConfigurationAsync(config);
            return success
                ? Ok(new { message = "Canteen configuration updated successfully" })
                : StatusCode(500, new { message = "Failed to update configuration" });
        }
    }
}
