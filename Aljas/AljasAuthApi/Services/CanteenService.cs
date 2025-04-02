using MongoDB.Driver;
using ProjectHierarchyApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectHierarchyApi.Services
{
    public class CanteenService
    {
      private readonly IMongoCollection<Canteen> _canteens;
private readonly IMongoCollection<Device> _devices; // Ensure you have this
private readonly IMongoCollection<Location> _locations; // Ensure you have this

public CanteenService(IMongoDatabase database)
{
    _canteens = database.GetCollection<Canteen>("Canteens");
    _devices = database.GetCollection<Device>("Devices");  // ✅ Correcting Device Collection
    _locations = database.GetCollection<Location>("Locations"); // ✅ Correcting Location Collection
}


        // Get Canteens by Location ID
        public async Task<List<Canteen>> GetCanteensByLocationIdAsync(string locationId) =>
            await _canteens.Find(c => c.LocationId == locationId).ToListAsync();

        // Add Canteen
      public async Task<bool> AddCanteenAsync(Canteen newCanteen)
{
    // Check if the Location exists and is active
    var location = await _locations.Find(l => l.Id == newCanteen.LocationId).FirstOrDefaultAsync();
    if (location == null || !location.Status) 
    {
        return false; // Canteen cannot be added to an inactive location
    }

    // Check if a canteen with the same name already exists under the same location
    var existingCanteen = await _canteens.Find(c => c.LocationId == newCanteen.LocationId && c.Name == newCanteen.Name).FirstOrDefaultAsync();
    if (existingCanteen != null)
    {
        return false; // Canteen with the same name already exists in this location
    }

    // If the Location is active and no duplicate canteen exists, add the new canteen
    await _canteens.InsertOneAsync(newCanteen);
    return true;
}


        // Update Canteen Status (Active/Inactive)
   public async Task<bool> UpdateCanteenAsync(string canteenId, Canteen updatedCanteen)
{
    // Find the existing canteen by ID
    var canteen = await _canteens.Find(c => c.Id == canteenId).FirstOrDefaultAsync();
    if (canteen == null) return false; // If no canteen is found, return false.

    // Ensure that the location is active before allowing the update (if this logic is needed)
    var location = await _locations.Find(l => l.Id == updatedCanteen.LocationId).FirstOrDefaultAsync();
    if (location != null && location.Status == false)
    {
        return false; // Prevent updating the canteen if the location is inactive
    }

    // Update all fields in the Canteen model
    updatedCanteen.Id = canteenId; // Ensure the ID remains the same

    // Create a filter to find the canteen by its ID and update it
    var filter = Builders<Canteen>.Filter.Eq(c => c.Id, canteenId);

    // Replace the existing Canteen with the updated one
    var result = await _canteens.ReplaceOneAsync(filter, updatedCanteen);

    // Return true if the update was successful (ModifiedCount > 0 means it was modified)
    return result.ModifiedCount > 0;
}

        // Delete a Canteen
     public async Task<bool> DeleteCanteenAsync(string id)
{
    // Check if any devices are linked to the canteen
    var deviceExists = await _devices.Find(d => d.CanteenId == id).AnyAsync();
    if (deviceExists)
        return false; // Cannot delete as devices are linked to this canteen

    // Proceed with deletion if no devices are linked
    var result = await _canteens.DeleteOneAsync(c => c.Id == id);
    return result.DeletedCount > 0;
}


        // Get Canteen by ID
        public async Task<Canteen?> GetCanteenByIdAsync(string canteenId) =>
            await _canteens.Find(c => c.Id == canteenId).FirstOrDefaultAsync();
    }
}
