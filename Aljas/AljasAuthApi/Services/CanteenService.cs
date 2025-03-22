using MongoDB.Driver;
using ProjectHierarchyApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectHierarchyApi.Services
{
    public class CanteenService
    {
        private readonly IMongoCollection<Canteen> _canteens;

        public CanteenService(IMongoDatabase database)
        {
            _canteens = database.GetCollection<Canteen>("Canteens");
        }

        public async Task<List<Canteen>> GetCanteensByLocationIdAsync(string locationId) =>
            await _canteens.Find(c => c.LocationId == locationId).ToListAsync();

        public async Task CreateCanteenAsync(Canteen canteen) =>
            await _canteens.InsertOneAsync(canteen);

      public async Task<bool> UpdateCanteenAsync(string id, Canteen updatedCanteen)
    {
    var filter = Builders<Canteen>.Filter.Eq(c => c.Id, id);
    var update = Builders<Canteen>.Update
        .Set(c => c.Name, updatedCanteen.Name)
        //.Set(c => c.ProjectId, updatedCanteen.ProjectId)
        .Set(c => c.LocationId, updatedCanteen.LocationId);

    var result = await _canteens.UpdateOneAsync(filter, update);
    return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteCanteenAsync(string id)
    {
    var result = await _canteens.DeleteOneAsync(c => c.Id == id);
    return result.DeletedCount > 0;
    }

        public async Task<Canteen?> GetCanteenByIdAsync(string canteenId) =>
    await _canteens.Find(c => c.Id == canteenId).FirstOrDefaultAsync();

    }
}
