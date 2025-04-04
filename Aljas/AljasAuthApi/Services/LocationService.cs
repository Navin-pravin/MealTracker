using MongoDB.Driver;
using ProjectHierarchyApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace ProjectHierarchyApi.Services
{
    public class LocationService
    {
        private readonly IMongoCollection<Location> _locations;
           private readonly IMongoCollection<Canteen> _canteens;


        public LocationService(IMongoDatabase database)
        {
            _locations = database.GetCollection<Location>("Locations");
               _canteens = database.GetCollection<Canteen>("Canteens");
        }

        public async Task<List<Location>> GetAllLocations() =>
          //  await _locations.Find(l => l.Id == Id).ToListAsync();
             await _locations.Find(_ => true).ToListAsync();

        public async Task CreateLocaionAsync(Location location) =>
            await _locations.InsertOneAsync(location);
public async Task<bool> UpdateLocationAsync(string id, Location updatedLocation)
        {
            var location = await _locations.Find(l => l.Id == id).FirstOrDefaultAsync();
            if (location == null) return false;

            // Check if the location is being deactivated, if so, check if there are active canteens
            if (!updatedLocation.Status)
            {
                // Find any active canteens associated with the location
                var activeCanteen = await _canteens.Find(c => c.LocationId == id && c.Status == true).FirstOrDefaultAsync();
                if (activeCanteen != null)
                {
                    // Cannot deactivate location if any of its canteens are still active
                    return false;
                }
            }

            // If the location is valid and no active canteens, update it
            var filter = Builders<Location>.Filter.Eq(l => l.Id, id);
            var result = await _locations.ReplaceOneAsync(filter, updatedLocation);

            return result.ModifiedCount > 0;
        }
    


       public async Task<bool> DeleteLocationAsync(string id)
{
    if (!ObjectId.TryParse(id, out ObjectId objectId))
        return false; // Invalid ID format

    var filter = Builders<Location>.Filter.Eq(l => l.Id, id);
    var result = await _locations.DeleteOneAsync(filter);

    return result.DeletedCount > 0;
}
public async Task<bool> HasActiveCanteens(string locationId)
{
    var activeCanteens = await _canteens.Find(c => c.LocationId == locationId && c.Status).AnyAsync();
    return activeCanteens;
}
public async Task<Location?> GetLocationByIdAsync(string id)
{
    return await _locations.Find(loc => loc.Id == id).FirstOrDefaultAsync();
}

    }
}
