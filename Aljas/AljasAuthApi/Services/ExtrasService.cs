using AljasAuthApi.Models;
using AljasAuthApi.Config;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace AljasAuthApi.Services
{
    public class ExtrasService
    {
        private readonly IMongoCollection<Department> _departments;
        private readonly IMongoCollection<Company> _companies;
        private readonly IMongoCollection<Designation> _designations;
        private readonly IMongoCollection<CLocation> _locations;
        private readonly IMongoCollection<MealConfiguration> _mealConfigurations;

        public ExtrasService(MongoDbSettings dbSettings)
        {
            var client = new MongoClient(dbSettings.ConnectionString);
            var database = client.GetDatabase(dbSettings.DatabaseName);
            _departments = database.GetCollection<Department>("Departments");
            _companies = database.GetCollection<Company>("Companies");
            _designations = database.GetCollection<Designation>("Designations");
            _locations = database.GetCollection<CLocation>("clocations");
            _mealConfigurations = database.GetCollection<MealConfiguration>("mealconfigurations");
        }

        // Department methods
        public async Task AddDepartmentAsync(Department department)
        {
            department.Id = ObjectId.GenerateNewId().ToString();
            await _departments.InsertOneAsync(department);
        }

        public async Task<bool> UpdateDepartmentAsync(string id, Department department)
        {
            var result = await _departments.ReplaceOneAsync(d => d.Id == id, department);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteDepartmentAsync(string id)
        {
            var result = await _departments.DeleteOneAsync(d => d.Id == id);
            return result.DeletedCount > 0;
        }

        public async Task<List<Department>> GetDepartmentSummaryAsync() =>
            await _departments.Find(_ => true).ToListAsync();

        // Company methods
        public async Task AddCompanyAsync(Company company) => 
            await _companies.InsertOneAsync(company);

        public async Task<bool> UpdateCompanyAsync(string id, Company company) =>
            (await _companies.ReplaceOneAsync(c => c.Id == id, company)).ModifiedCount > 0;

        public async Task<bool> DeleteCompanyAsync(string id) =>
            (await _companies.DeleteOneAsync(c => c.Id == id)).DeletedCount > 0;

        public async Task<List<Company>> GetCompanySummaryAsync() =>
            await _companies.Find(_ => true).ToListAsync();

        // Designation methods
        public async Task AddDesignationAsync(Designation designation) =>
            await _designations.InsertOneAsync(designation);

        public async Task<bool> UpdateDesignationAsync(string id, Designation designation) =>
            (await _designations.ReplaceOneAsync(d => d.Id == id, designation)).ModifiedCount > 0;

        public async Task<bool> DeleteDesignationAsync(string id) =>
            (await _designations.DeleteOneAsync(d => d.Id == id)).DeletedCount > 0;

        public async Task<List<Designation>> GetDesignationSummaryAsync() =>
            await _designations.Find(_ => true).ToListAsync();

        // Location methods
        public async Task AddLocationAsync(CLocation location) =>
            await _locations.InsertOneAsync(location);

        public async Task<bool> UpdateLocationAsync(string id, CLocation location) =>
            (await _locations.ReplaceOneAsync(d => d.Id == id, location)).ModifiedCount > 0;

        public async Task<bool> DeleteLocationAsync(string id) =>
            (await _locations.DeleteOneAsync(d => d.Id == id)).DeletedCount > 0;

        public async Task<List<CLocation>> GetLocationSummaryAsync() =>
            await _locations.Find(_ => true).ToListAsync();

        // Meal Configuration methods
        public async Task AddMealConfigurationAsync(MealConfiguration config) =>
            await _mealConfigurations.InsertOneAsync(config);

        public async Task<bool> UpdateMealConfigurationAsync(string id, MealConfiguration config) =>
            (await _mealConfigurations.ReplaceOneAsync(m => m.Id == id, config)).ModifiedCount > 0;

        public async Task<bool> DeleteMealConfigurationAsync(string id) =>
            (await _mealConfigurations.DeleteOneAsync(m => m.Id == id)).DeletedCount > 0;

        public async Task<List<MealConfiguration>> GetMealConfigurationsAsync() =>
            await _mealConfigurations.Find(_ => true).ToListAsync();

        public async Task<List<MealConfiguration>> GetActiveMealConfigurationsAsync() =>
            await _mealConfigurations.Find(m => m.IsActive).ToListAsync();
    }
}