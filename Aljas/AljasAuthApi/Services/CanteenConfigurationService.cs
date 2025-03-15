using MongoDB.Driver;
using ProjectHierarchyApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectHierarchyApi.Services
{
    public class CanteenConfigurationService
    {
        private readonly IMongoCollection<CanteenConfiguration> _configurations;

        public CanteenConfigurationService(IMongoDatabase database)
        {
            _configurations = database.GetCollection<CanteenConfiguration>("CanteenConfigurations");
        }

        // ✅ Get configuration by Canteen ID
        public async Task<CanteenConfiguration?> GetConfigurationByCanteenIdAsync(string canteenId) =>
            await _configurations.Find(config => config.CanteenId == canteenId).FirstOrDefaultAsync();

        // ✅ Insert or update canteen configuration
        public async Task<bool> UpsertConfigurationAsync(CanteenConfiguration config)
        {
            var existingConfig = await GetConfigurationByCanteenIdAsync(config.CanteenId);
            if (existingConfig != null)
            {
                var update = Builders<CanteenConfiguration>.Update.Set(c => c.MealTimings, config.MealTimings);
                var result = await _configurations.UpdateOneAsync(c => c.CanteenId == config.CanteenId, update);
                return result.ModifiedCount > 0;
            }
            else
            {
                await _configurations.InsertOneAsync(config);
                return true;
            }
        }
    }
}
