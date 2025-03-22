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

        public LocationService(IMongoDatabase database)
        {
            _locations = database.GetCollection<Location>("Locations");
        }

        public async Task<List<Location>> GetLocationsByProjectIdAsync(string projectId) =>
            await _locations.Find(l => l.ProjectId == projectId).ToListAsync();

        public async Task CreateLocationAsync(Location location) =>
            await _locations.InsertOneAsync(location);

       public async Task<bool> UpdateLocationAsync(string id, Location updatedLocation)
{
    if (!ObjectId.TryParse(id, out ObjectId objectId))
        return false; // Invalid ID format

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

    }
}
