using Microsoft.AspNetCore.Mvc;
using ProjectHierarchyApi.Models;
using ProjectHierarchyApi.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
namespace ProjectHierarchyApi.Controllers
{
    [Route("api/locations")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly LocationService _locationService;
          private readonly IMongoCollection<Canteen> _canteens; // Mongo collection for canteens
        private readonly IMongoCollection<Location> _locations;

        public LocationController(LocationService locationService, IMongoDatabase database)
        {
            _locationService = locationService;
             _locations = database.GetCollection<Location>("Locations");
            _canteens = database.GetCollection<Canteen>("Canteens");
        }

        // ✅ Get all locations for a given project
        [HttpGet("location-summary")]
        public async Task<ActionResult<List<Location>>> GetAllLocations() =>
        
             await _locationService.GetAllLocations();
            
        

        // ✅ Create a new location under a project
        [HttpPost("add-location")]
        public async Task<IActionResult> CreateLocation(Location location)
        {
            await _locationService.CreateLocaionAsync(location);
            return Ok(new { message = "Location created successfully" });
        }

 // Endpoint to update a location
        [HttpPut("update-location/{id}")]
        public async Task<IActionResult> UpdateLocation(string id, [FromBody] Location updatedLocation)
        {
            // Check if the Location ID format is valid
            if (!ObjectId.TryParse(id, out _))
                return BadRequest(new { message = "Invalid Location ID format." });

            // Update the location using the service
            var success = await _locationService.UpdateLocationAsync(id, updatedLocation);

            if (!success)
                return NotFound(new { message = "Location not found or cannot deactivate due to active canteens." });

            return Ok(new { message = "Location updated successfully" });
        }
        // ✅ Delete a location
       [HttpDelete("delete-location/{id}")]
public async Task<IActionResult> DeleteLocation(string id)
{
    // Validate if the provided location ID is valid
    if (!ObjectId.TryParse(id, out _))
        return BadRequest(new { message = "Invalid Location ID format." });

    // Check if the location has any associated canteens
    var canteens = await _canteens.Find(c => c.LocationId == id).ToListAsync();
    if (canteens.Count > 0)
    {
        return BadRequest(new { message = "Cannot delete location because it has associated canteens." });
    }

    // Proceed with deletion if no canteens are associated
    var success = await _locationService.DeleteLocationAsync(id);
    if (!success)
        return NotFound(new { message = "Location not found" });

    return Ok(new { message = "Location deleted successfully" });
}

    }
}
