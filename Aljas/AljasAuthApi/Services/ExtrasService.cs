using AljasAuthApi.Models;
using AljasAuthApi.Config;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AljasAuthApi.Services
{
    public class ExtrasService
    {
        private readonly IMongoCollection<Department> _departments;
        private readonly IMongoCollection<Company> _companies;
        private readonly IMongoCollection<Role> _roles;
        private readonly IMongoCollection<Designation> _designations;
        private readonly IMongoCollection<CLocation> _locations;

        public ExtrasService(MongoDbSettings dbSettings)
        {

            var client = new MongoClient(dbSettings.ConnectionString);
            var database = client.GetDatabase(dbSettings.DatabaseName);
            _departments = database.GetCollection<Department>("Departments");
            _companies = database.GetCollection<Company>("Companies");
            _roles = database.GetCollection<Role>("Roles");
            _designations = database.GetCollection<Designation>("Designations");
            _locations = database.GetCollection<CLocation>("clocations");

        }

        // ✅ Department Service Methods
        public async Task AddDepartmentAsync(Department department) => await _departments.InsertOneAsync(department);
        public async Task<bool> UpdateDepartmentAsync(string id, Department department) =>
            (await _departments.ReplaceOneAsync(d => d.Id == id, department)).ModifiedCount > 0;
        public async Task<bool> DeleteDepartmentAsync(string id) =>
            (await _departments.DeleteOneAsync(d => d.Id == id)).DeletedCount > 0;
        public async Task<List<Department>> GetDepartmentSummaryAsync() => await _departments.Find(_ => true).ToListAsync();

        // ✅ Company Service Methods
        public async Task AddCompanyAsync(Company company) => await _companies.InsertOneAsync(company);
        public async Task<bool> UpdateCompanyAsync(string id, Company company) =>
            (await _companies.ReplaceOneAsync(c => c.Id == id, company)).ModifiedCount > 0;
        public async Task<bool> DeleteCompanyAsync(string id) =>
            (await _companies.DeleteOneAsync(c => c.Id == id)).DeletedCount > 0;
        public async Task<List<Company>> GetCompanySummaryAsync() => await _companies.Find(_ => true).ToListAsync();

        // ✅ Role Service Methods
        public async Task AddRoleAsync(Role role) => await _roles.InsertOneAsync(role);
        public async Task<bool> UpdateRoleAsync(string id, Role role) =>
            (await _roles.ReplaceOneAsync(r => r.Id == id, role)).ModifiedCount > 0;
        public async Task<bool> DeleteRoleAsync(string id) =>
            (await _roles.DeleteOneAsync(r => r.Id == id)).DeletedCount > 0;
        public async Task<List<Role>> GetRoleSummaryAsync() => await _roles.Find(_ => true).ToListAsync();

        // ✅ Designation Service Methods
        public async Task AddDesignationAsync(Designation designation) => await _designations.InsertOneAsync(designation);
        public async Task<bool> UpdateDesignationAsync(string id, Designation designation) =>
            (await _designations.ReplaceOneAsync(d => d.Id == id, designation)).ModifiedCount > 0;
        public async Task<bool> DeleteDesignationAsync(string id) =>
            (await _designations.DeleteOneAsync(d => d.Id == id)).DeletedCount > 0;
        public async Task<List<Designation>> GetDesignationSummaryAsync() => await _designations.Find(_ => true).ToListAsync();

        public async Task AddlocationAsync(CLocation location) => await _locations.InsertOneAsync(location);
        public async Task<bool> UpdatelocationAsync(string id, CLocation location) =>
            (await _locations.ReplaceOneAsync(d => d.Id == id, location)).ModifiedCount > 0;
        public async Task<bool> DeletelocationAsync(string id) =>
            (await _locations.DeleteOneAsync(d => d.Id == id)).DeletedCount > 0;
        public async Task<List<CLocation>> GetlocationSummaryAsync() => await _locations.Find(_ => true).ToListAsync();
    }
}
