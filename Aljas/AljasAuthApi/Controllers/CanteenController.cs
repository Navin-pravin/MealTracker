using Microsoft.AspNetCore.Mvc;
using ProjectHierarchyApi.Models;
using ProjectHierarchyApi.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
namespace ProjectHierarchyApi.Controllers

{
    [Route("api/canteens")]
    [ApiController]
     public class CanteenController : ControllerBase
    {
        private readonly CanteenService _canteenService;
        private readonly IMongoCollection<Location> _locations; // Add the Locations collection here

        // Inject the dependencies in the constructor
        public CanteenController(CanteenService canteenService, IMongoDatabase database)
        {
            _canteenService = canteenService;
            _locations = database.GetCollection<Location>("Locations"); // Initialize _locations collection
        }
        // ✅ Get all canteens for a given project and location
        [HttpGet("canteen-summary")]
        public async Task<ActionResult<List<Canteen>>> GetCanteensByLocation( string locationId)
        {
            var canteens = await _canteenService.GetCanteensByLocationIdAsync(locationId);
            return Ok(canteens);
        }

        // ✅ Create a new canteen under a project and location
        
        [HttpPost("add-canteen")]
        public async Task<IActionResult> AddCanteen([FromBody] Canteen newCanteen)
        {
            bool success = await _canteenService.AddCanteenAsync(newCanteen);
            if (!success)
            {
                return BadRequest(new { message = "Canteen cannot be added because the location is inactive or canteen name already exist." });
            }

            return Ok(new { message = "Canteen added successfully." });
        }
       // Endpoint to update canteen status
       // ✅ Update a canteen status or all canteen fields
[HttpPut("update-canteen/{canteenId}")]
public async Task<IActionResult> UpdateCanteen(string canteenId, [FromBody] Canteen updatedCanteen)
{
    // If the location is inactive, don't allow the update
    var location = await _locations.Find(l => l.Id == updatedCanteen.LocationId).FirstOrDefaultAsync();
    if (location != null && !location.Status)
    {
        return BadRequest(new { message = "Cannot update canteen because the location is inactive." });
    }

    // Update the canteen in the database
    bool success = await _canteenService.UpdateCanteenAsync(canteenId, updatedCanteen);
    if (!success) 
        return NotFound(new { message = "Canteen not found" });

    return Ok(new { message = "Canteen updated successfully" });
}


// ✅ Delete a canteen
[HttpDelete("delete-canteen/{id}")]
public async Task<IActionResult> DeleteCanteen(string id)
{
    if (!ObjectId.TryParse(id, out _))
        return BadRequest(new { message = "Invalid Canteen ID format." });

    var success = await _canteenService.DeleteCanteenAsync(id);
    if (!success) 
        return BadRequest(new { message = "Canteen cannot be deleted as it has linked devices." });

    return Ok(new { message = "Canteen deleted successfully" });
}


    }
}
