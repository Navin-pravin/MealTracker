using AljasAuthApi.Config;
using AljasAuthApi.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using MongoDB.Bson;

namespace AljasAuthApi.Services
{
    public class EmployeeService
    {
        private readonly IMongoCollection<Employee> _employees;
        private readonly IMongoCollection<EmployeeUploadError> _employeeUploadErrors;

          private readonly RoleHierarchyService _roleHierarchyService;

        private readonly ExtrasService _extrasService;

        public EmployeeService(MongoDbSettings settings, ExtrasService extrasService, RoleHierarchyService rolehierarchy)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _employees = database.GetCollection<Employee>(settings.EmployeesCollectionName);
            _employeeUploadErrors = database.GetCollection<EmployeeUploadError>("EmployeeUploadErrors");
            _extrasService = extrasService;
            _roleHierarchyService = rolehierarchy;
        }

        public async Task<List<Employee>> GetAllEmployeesAsync(string? Firstname = null, string? Dept = null)
        {
            var filter = Builders<Employee>.Filter.Empty;

            if (!string.IsNullOrEmpty(Firstname))
                filter &= Builders<Employee>.Filter.Regex("Firstname", new BsonRegularExpression(Firstname, "i"));

            if (!string.IsNullOrEmpty(Dept))
                filter &= Builders<Employee>.Filter.Eq(emp => emp.Dept, Dept);

            return await _employees.Find(filter).ToListAsync();
        }

        public async Task<Employee?> GetEmployeeByIdAsync(string id) =>
            await _employees.Find(emp => emp.Id == id).FirstOrDefaultAsync();
public async Task<List<Employee>?> GetEmployeeByDeptAsync(string dept)
{
    // Find all employees in the department
    return await _employees.Find(emp => emp.Dept == dept).ToListAsync();
}

        public async Task<bool> CreateEmployeeAsync(Employee employee)
        {
            if (employee == null) return false;

            var existingEmployee = await _employees.Find(emp => emp.IDNumber == employee.IDNumber).FirstOrDefaultAsync();
            if (existingEmployee != null)
            {
                Console.WriteLine($"❌ Duplicate IDNumber: {employee.IDNumber} already exists.");
                return false;
            }

            employee.Id = ObjectId.GenerateNewId().ToString();
            try
            {
                await _employees.InsertOneAsync(employee);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Failed to create employee: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateEmployeeAsync(string id, Employee updatedEmployee)
        {
            var result = await _employees.ReplaceOneAsync(emp => emp.Id == id, updatedEmployee);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteEmployeeAsync(string id)
        {
            var result = await _employees.DeleteOneAsync(emp => emp.Id == id);
            return result.DeletedCount > 0;
        }

        public async Task<(bool success, List<EmployeeUploadError> errors)> BulkUploadEmployeesFromExcelAsync(List<Employee> employees)
        {
            if (employees == null || employees.Count == 0)
                return (false, new List<EmployeeUploadError>());

            var validEmployees = new List<Employee>();
            var errorReports = new List<EmployeeUploadError>();

            var validDepartments = (await _extrasService.GetDepartmentSummaryAsync()).ConvertAll(d => d.DepartmentName.ToLower());
            var validCompanies = (await _extrasService.GetCompanySummaryAsync()).ConvertAll(c => c.CompanyName.ToLower());
           // var validRoles = (await _extrasService.GetRoleSummaryAsync()).ConvertAll(r => r.RoleTitle.ToLower());
            var validDesignations = (await _extrasService.GetDesignationSummaryAsync()).ConvertAll(d => d.DesignationTitle.ToLower());
            var validLocations = (await _extrasService.GetLocationSummaryAsync()).ConvertAll(l => l.location.ToLower());

            int rowNumber = 2;

            var existingEmployees = await _employees.Find(_ => true).Project(e => e.IDNumber).ToListAsync();
            var existingIDNumbers = new HashSet<string>(existingEmployees);
            var newIDNumbers = new HashSet<string>();

            foreach (var employee in employees)
            {
                var fieldErrors = new Dictionary<string, string>();

                if (existingIDNumbers.Contains(employee.IDNumber))
                {
                    fieldErrors["IDNumber"] = $"Duplicate IDNumber: {employee.IDNumber} already exists in database.";
                }
                else if (!newIDNumbers.Add(employee.IDNumber))
                {
                    fieldErrors["IDNumber"] = $"Duplicate IDNumber: {employee.IDNumber} appears multiple times in the uploaded file.";
                }

                if (!fieldErrors.ContainsKey("IDNumber"))
                {
                    if (!validDepartments.Contains(employee.Dept.ToLower()))
                        fieldErrors["Dept"] = $"Invalid Department: {employee.Dept}";
                    
                    if (!validCompanies.Contains(employee.Company.ToLower()))
                        fieldErrors["Company"] = $"Invalid Company: {employee.Company}";
                    
                    //if (!validRoles.Contains(employee.Role.ToLower()))
                      //  fieldErrors["Role"] = $"Invalid Role: {employee.Role}";
                    
                    if (!validDesignations.Contains(employee.designation.ToLower()))
                        fieldErrors["Designation"] = $"Invalid Designation: {employee.designation}";
                    
                    if (!validLocations.Contains(employee.location.ToLower()))
                        fieldErrors["Location"] = $"Invalid Location: {employee.location}";
                }

                if (fieldErrors.Count == 0)
                {
                    employee.Id = Guid.NewGuid().ToString();
                    validEmployees.Add(employee);
                }
                else
                {
                    errorReports.Add(new EmployeeUploadError
                    {
                        Id = ObjectId.GenerateNewId().ToString(),
                        RowNumber = rowNumber,
                        EmployeeData = employee,
                        Errors = fieldErrors
                    });
                }
                rowNumber++;
            }

            if (validEmployees.Count > 0)
            {
                await _employees.InsertManyAsync(validEmployees);
            }

            if (errorReports.Count > 0)
            {
                await _employeeUploadErrors.InsertManyAsync(errorReports);
            }

            return (validEmployees.Count > 0, errorReports);
        }

        public async Task<List<EmployeeUploadError>> GetUploadErrorsAsync()
        {
            return await _employeeUploadErrors.Find(_ => true).ToListAsync();
        }
     public async Task<List<Employee>> GetEmployeesByCanteenIdAsync(string canteenId)
{
    var roleNames = await _roleHierarchyService.GetRoleNamesByCanteenIdAsync(canteenId);

    var filter = Builders<Employee>.Filter.In(e => e.Role, roleNames);
    return await _employees.Find(filter).ToListAsync();
}


    }
}
