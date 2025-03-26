using Microsoft.AspNetCore.Mvc;
using ProjectHierarchyApi.Models;
using ProjectHierarchyApi.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace ProjectHierarchyApi.Controllers
{
    [Route("api/locations")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly LocationService _locationService;

        public LocationController(LocationService locationService)
        {
            _locationService = locationService;
        }

        // ✅ Get all locations for a given project
        [HttpGet("location-summary")]
        public async Task<ActionResult<List<Location>>> GetAllLocations() =>
        
             await _locationService.GetAllLocations();
            
        

        // ✅ Create a new location under a project
        [HttpPost("add-location")]
        public async Task<IActionResult> CreateLocation(Location location)
        {
            await _locationService.CreateLocationAsync(location);
            return Ok(new { message = "Location created successfully" });
        }

        // ✅ Update a location
        [HttpPut("update-location/{id}")]
public async Task<IActionResult> UpdateLocation(string id, [FromBody] Location updatedLocation)
{
    if (!ObjectId.TryParse(id, out _))
        return BadRequest(new { message = "Invalid Location ID format." });

    var success = await _locationService.UpdateLocationAsync(id, updatedLocation);
    if (!success)
        return NotFound(new { message = "Location not found" });

    return Ok(new { message = "Location updated successfully" });
}

        // ✅ Delete a location
        [HttpDelete("delete-location/{id}")]
public async Task<IActionResult> DeleteLocation(string id)
{
    if (!ObjectId.TryParse(id, out _))
        return BadRequest(new { message = "Invalid Location ID format." });

    var success = await _locationService.DeleteLocationAsync(id);
    if (!success) return NotFound(new { message = "Location not found" });

    return Ok(new { message = "Location deleted successfully" });
}

    }
}
