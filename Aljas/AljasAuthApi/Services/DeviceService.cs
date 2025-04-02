using ProjectHierarchyApi.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectHierarchyApi.Services
{
    public class DeviceService
    {
        private readonly IMongoCollection<Device> _devices;
        private readonly IMongoCollection<Project> _projects;
        private readonly IMongoCollection<Location> _locations;
        private readonly IMongoCollection<Canteen> _canteens;

        public DeviceService(IMongoDatabase database)
        {
            _devices = database.GetCollection<Device>("Devices");
            _projects = database.GetCollection<Project>("Projects");
            _locations = database.GetCollection<Location>("Locations");
            _canteens = database.GetCollection<Canteen>("Canteens");
        }

        public async Task<List<Device>> GetDevicesAsync() => await _devices.Find(device => true).ToListAsync();

        public async Task<Device?> GetDeviceByUniqueIdAsync(string uniqueId) =>
            await _devices.Find(device => device.UniqueId == uniqueId).FirstOrDefaultAsync();

       public async Task<bool> CreateDeviceAsync(Device device)
{
    var location = await _locations.Find(l => l.Name == device.LocationName).FirstOrDefaultAsync();
    var canteen = await _canteens.Find(c => c.Name == device.CanteenName).FirstOrDefaultAsync();

    if (location == null || canteen == null)
        return false; // Location or Canteen not found

    // Check for duplicate UniqueId
    var existingDevice = await GetDeviceByUniqueIdAsync(device.UniqueId);
    if (existingDevice != null)
        return false;

    // Store IDs along with names
    device.LocationId = location.Id;
    device.LocationName = location.Name;
    device.CanteenId = canteen.Id;
    device.CanteenName = canteen.Name;

    await _devices.InsertOneAsync(device);
    return true;
}

public async Task<bool> UpdateDeviceByUniqueIdAsync(string uniqueId, Device updatedDevice)
{
    var existingDevice = await GetDeviceByUniqueIdAsync(uniqueId);
    if (existingDevice == null) return false;

    var location = await _locations.Find(l => l.Name == updatedDevice.LocationName).FirstOrDefaultAsync();
    var canteen = await _canteens.Find(c => c.Name == updatedDevice.CanteenName).FirstOrDefaultAsync();

    if (location == null || canteen == null)
        return false; // Location or Canteen not found

    updatedDevice.LocationId = location.Id;
    updatedDevice.LocationName = location.Name;
    updatedDevice.CanteenId = canteen.Id;
    updatedDevice.CanteenName = canteen.Name;

    var result = await _devices.ReplaceOneAsync(device => device.UniqueId == uniqueId, updatedDevice);
    return result.ModifiedCount > 0;
}

        public async Task<bool> DeleteDeviceByUniqueIdAsync(string uniqueId)
        {
            var result = await _devices.DeleteOneAsync(device => device.UniqueId == uniqueId);
            return result.DeletedCount > 0;
        }
    }
}
