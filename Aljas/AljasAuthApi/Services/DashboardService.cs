using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AljasAuthApi.Models;
using System.Linq;
using ProjectHierarchyApi.Models;

namespace AljasAuthApi.Services
{
    public class DashboardService
    {
        private readonly IMongoCollection<RoleHierarchy> _roleHierarchy;
        private readonly IMongoCollection<RawData> _rawData;
        private readonly IMongoCollection<Dashboard> _Dashboard;
        private readonly IMongoCollection<Device> _devices;

        public DashboardService(MongoDbSettings dbSettings)
        {
            var client = new MongoClient(dbSettings.ConnectionString);
            var database = client.GetDatabase(dbSettings.DatabaseName);
            _Dashboard = database.GetCollection<Dashboard>("dashboard");
            _roleHierarchy = database.GetCollection<RoleHierarchy>("RoleHierarchy");
            _rawData = database.GetCollection<RawData>("rawdata");
            _devices = database.GetCollection<Device>("Devices");
        }

        public async Task<List<string>> GetMealTypesByLocationAndCanteenAsync(string locationId, string canteenId)
        {
            var filter = Builders<RoleHierarchy>.Filter.ElemMatch(r => r.Locations,
                loc => loc.LocationId == locationId && loc.Canteens.Any(c => c.CanteenId == canteenId));

            var roleHierarchyList = await _roleHierarchy.Find(filter).ToListAsync();

            var mealTypes = new List<string>();

            foreach (var role in roleHierarchyList)
            {
                foreach (var location in role.Locations)
                {
                    if (location.LocationId == locationId)
                    {
                        foreach (var canteen in location.Canteens)
                        {
                            if (canteen.CanteenId == canteenId)
                            {
                                foreach (var meal in canteen.MealConfigurations)
                                {
                                    if (meal.IsActive)
                                    {
                                        mealTypes.Add(meal.MealType);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return mealTypes.Distinct().ToList(); // remove duplicates
        }

        public async Task CreateDashboardAsync(Dashboard dashboard)
        {
            await _Dashboard.InsertOneAsync(dashboard);
        }

        public async Task<List<Dashboard>> GetAllDashboardsAsync()
        {
            return await _Dashboard.Find(_ => true).ToListAsync();
        }

        public async Task<List<DashboardMealCountSummary>> GetDashboardMealCountsAsync()
{
    var now = DateTime.UtcNow;
    var startOfDay = now.Date;
    var startOfWeek = now.Date.AddDays(-(int)now.DayOfWeek);
    var startOfMonth = new DateTime(now.Year, now.Month, 1);

    var dashboards = await _Dashboard.Find(_ => true).ToListAsync();
    var rawData = await _rawData.Find(_ => true).ToListAsync();
    var devices = await _devices.Find(_ => true).ToListAsync();
    var roleHierarchies = await _roleHierarchy.Find(_ => true).ToListAsync();

    var result = new List<DashboardMealCountSummary>();

    foreach (var dashboard in dashboards)
    {
        var matchedRoles = roleHierarchies
            .Where(r => r.Locations
                .Any(l => l.Canteens.Any(c => c.CanteenId == dashboard.CanteenId)))
            .ToList();

        var mealTypes = matchedRoles
            .SelectMany(r => r.Locations)
            .Where(l => l.Canteens.Any(c => c.CanteenId == dashboard.CanteenId))
            .SelectMany(l => l.Canteens
                .Where(c => c.CanteenId == dashboard.CanteenId)
                .SelectMany(c => c.MealConfigurations))
            .Where(m => m.IsActive)
            .Select(m => m.MealType)
            .Distinct()
            .ToList();

        var matchingDevices = devices
            .Where(d => d.CanteenId == dashboard.CanteenId)
            .Select(d => d.UniqueId)
            .ToHashSet();

        var filteredRawData = rawData
            .Where(r => matchingDevices.Contains(r.Device_uniqueid) && mealTypes.Contains(r.MealType))
            .ToList();

        var today = mealTypes.ToDictionary(
            m => m,
            m => filteredRawData.Count(r => r.CreatedDateUtc.Date == startOfDay && r.MealType == m)
        );

        var thisWeek = mealTypes.ToDictionary(
            m => m,
            m => filteredRawData.Count(r => r.CreatedDateUtc >= startOfWeek && r.MealType == m)
        );

        var thisMonth = mealTypes.ToDictionary(
            m => m,
            m => filteredRawData.Count(r => r.CreatedDateUtc >= startOfMonth && r.MealType == m)
        );

        // ✅ Calculate Totals
        var totalToday = today.Values.Sum();
        var totalThisWeek = thisWeek.Values.Sum();
        var totalThisMonth = thisMonth.Values.Sum();

        result.Add(new DashboardMealCountSummary
        {
            Dashboardid = dashboard.Id,
            DashboardName = dashboard.DashboardName,
            LocationId = dashboard.LocationId,
            CanteenId = dashboard.CanteenId,
            MealTypeCounts = new MealCountSummary
            {
                Today = today,
                ThisWeek = thisWeek,
                ThisMonth = thisMonth
            },
            TotalToday = totalToday,
            TotalThisWeek = totalThisWeek,
            TotalThisMonth = totalThisMonth
        });
    }

    return result;
}
public async Task<bool> DeleteDashboardByIdAsync(string id)
{
    var result = await _Dashboard.DeleteOneAsync(d => d.Id == id);
    return result.DeletedCount > 0;
}

    }
}
