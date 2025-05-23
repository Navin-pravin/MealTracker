using Microsoft.AspNetCore.Mvc;
using AljasAuthApi.Models;
using AljasAuthApi.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System;
using OfficeOpenXml;
using System.IO;

namespace AljasAuthApi.Controllers
{
    [Route("api/employees")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly EmployeeService _employeeService;
        private readonly RoleHierarchyService _roleHierarchyService;

        public EmployeeController(EmployeeService employeeService ,RoleHierarchyService rolehierarchy)
        {
            _employeeService = employeeService;
            _roleHierarchyService=rolehierarchy;
        }

       [HttpGet("Employee-Summary")]
public async Task<IActionResult> GetAllEmployees([FromQuery] string? clientid)
{
    // ✅ Define allowed Client ID (Set your actual value)
    string allowedClientId = "c01";

    // ✅ Validate the client ID
    if (string.IsNullOrEmpty(clientid) || clientid != allowedClientId)
    {
        return Unauthorized(new { message = "Invalid Client ID." });
    }

    // ✅ Fetch employees only if the client ID matches
    var employees = await _employeeService.GetAllEmployeesAsync();
    return Ok(employees);
}


        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeById(string id)
        {
    var employee = await _employeeService.GetEmployeeByIdAsync(id);
    if (employee == null)
        return NotFound(new { message = "Employee not found" });

    // ✅ Restrict access if employee status is "Inactive"
    if (employee.Status == true)
        return BadRequest(new { message = "Access denied. Employee is inactive." });

    return Ok(employee);
}
 [HttpGet("department/{dept}")]
        public async Task<IActionResult> GetEmployeeByDept(string dept)
        {
    var employee = await _employeeService.GetEmployeeByDeptAsync(dept);
 if (employee== null || !employee.Any())
        return NotFound(new { message = "No employees found for the specified department." });

    // ✅ Restrict access if employee status is "Inactive"
  //  if (employee.Status == true)
    //    return BadRequest(new { message = "Access denied. Employee is inactive." });

    return Ok(employee);
}


        [HttpPost("add-employee")]
public async Task<IActionResult> CreateEmployee([FromBody] Employee? employee)
{
    if (employee == null)
        return BadRequest(new { message = "Invalid employee data." });

    bool created = await _employeeService.CreateEmployeeAsync(employee);
    if (!created)
        return StatusCode(500, new { message = "Failed to create employee." });

    return Ok(new 
    { 
        message = "Employee created successfully", 
        employee = employee 
    });
}


        [HttpPut("update-employee/{id}")]
        public async Task<IActionResult> UpdateEmployee(string id, [FromBody] Employee? updatedEmployee)
        {
            if (updatedEmployee == null)
                return BadRequest(new { message = "Invalid employee data." });

            bool success = await _employeeService.UpdateEmployeeAsync(id, updatedEmployee);
            if (!success)
                return NotFound(new { message = "Employee not found" });

            return Ok(new { message = "Employee updated successfully" });
        }

        [HttpDelete("delete-employee/{id}")]
        public async Task<IActionResult> DeleteEmployee(string id)
        {
            bool success = await _employeeService.DeleteEmployeeAsync(id);
            if (!success)
                return NotFound(new { message = "Employee not found" });

            return Ok(new { message = "Employee deleted successfully" });
        }
       [HttpPost("upload-excel")]
public async Task<IActionResult> UploadEmployeesFromExcel(IFormFile file)
{
    if (file == null || file.Length == 0)
        return BadRequest(new { message = "No file uploaded." });

    if (file.ContentType != "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
        return BadRequest(new { message = "Invalid file format. Please upload a valid Excel file." });

    try
    {
        var employees = new List<Employee>();

        using (var stream = new MemoryStream())
        {
            await file.CopyToAsync(stream);
            stream.Position = 0;

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets[0];
                int rowCount = worksheet.Dimension?.Rows ?? 0;

                if (rowCount < 2)
                    return BadRequest(new { message = "Excel file is empty or missing headers." });

                for (int row = 2; row <= rowCount; row++)
                {
                    try
                    {
                        var employee = new Employee
                        {
                            Id = Guid.NewGuid().ToString(),
                            IDNumber = worksheet.Cells[row, 1]?.Text?.Trim() ?? "",
                            Firstname = worksheet.Cells[row, 2]?.Text?.Trim() ?? "",
                            Lastname = worksheet.Cells[row, 3]?.Text?.Trim() ?? "",
                            Email=worksheet.Cells[row,4]?.Text?.Trim() ??"",
                            StartDate = worksheet.Cells[row, 5]?.Text?.Trim() ?? "",
                            EndDate = worksheet.Cells[row, 6]?.Text?.Trim() ?? "",
                            Phone_no = worksheet.Cells[row, 7]?.Text?.Trim() ?? "",
                            Dept = worksheet.Cells[row, 8]?.Text?.Trim() ?? "",
                            Role = worksheet.Cells[row, 9]?.Text?.Trim() ?? "",
                            designation = worksheet.Cells[row, 10]?.Text?.Trim() ?? "",
                            imageUrl = worksheet.Cells[row, 11]?.Text?.Trim() ?? "",
                            Company = worksheet.Cells[row, 12]?.Text?.Trim() ?? "",
                            location = worksheet.Cells[row, 13]?.Text?.Trim() ?? "",
                            Referenceid = worksheet.Cells[row, 14]?.Text?.Trim() ?? "",
                            CardBadgeNumber = worksheet.Cells[row, 15]?.Text?.Trim() ?? "",
                           Status = bool.TryParse(worksheet.Cells[row, 16]?.Text?.Trim(), out bool result) ? result : true

                        };

                        employees.Add(employee);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"❌ Error processing row {row}: {ex.Message}");
                    }
                }
            }
        }

        // ✅ Correct the method call to match the expected return type
        (bool success, List<EmployeeUploadError> errors) = await _employeeService.BulkUploadEmployeesFromExcelAsync(employees);

        if (errors.Count > 0)
            return BadRequest(new { message = "Some records were invalid. Download the error report.", errors });

        return Ok(new { message = $"{employees.Count} employees uploaded successfully." });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Upload failed: {ex}");
        return StatusCode(500, new { message = "Error processing the Excel file.", details = ex.Message });
    }
}


        // ✅ Sample Excel Download API (No async needed)
       [HttpGet("download-error-report")]
public async Task<IActionResult> DownloadErrorReport()
{
    var errorReports = await _employeeService.GetUploadErrorsAsync();
    
    if (errorReports == null || errorReports.Count == 0)
        return NotFound(new { message = "No errors found." });

    using var package = new ExcelPackage();
    var worksheet = package.Workbook.Worksheets.Add("UploadErrors");

    // ✅ Set up headers
    worksheet.Cells[1, 1].Value = "RowNumber";
    worksheet.Cells[1, 2].Value = "IDNumber";
    worksheet.Cells[1, 3].Value = "Firstname";
    worksheet.Cells[1, 4].Value = "Lastname";
    worksheet.Cells[1, 5].Value = "Dept";
    worksheet.Cells[1, 6].Value = "Role";
    worksheet.Cells[1, 7].Value = "Designation";
    worksheet.Cells[1, 8].Value = "Company";
    worksheet.Cells[1, 9].Value = "Location";
    worksheet.Cells[1, 10].Value = "Errors";

    int row = 2;
    foreach (var error in errorReports)
    {
        worksheet.Cells[row, 1].Value = error.RowNumber;
        worksheet.Cells[row, 2].Value = error.EmployeeData.IDNumber;
        worksheet.Cells[row, 3].Value = error.EmployeeData.Firstname;
        worksheet.Cells[row, 4].Value = error.EmployeeData.Lastname;
        worksheet.Cells[row, 5].Value = error.EmployeeData.Dept;
        worksheet.Cells[row, 6].Value = error.EmployeeData.Role;
        worksheet.Cells[row, 7].Value = error.EmployeeData.designation;
        worksheet.Cells[row, 8].Value = error.EmployeeData.Company;
        worksheet.Cells[row, 9].Value = error.EmployeeData.location;
        worksheet.Cells[row, 10].Value = string.Join("; ", error.Errors.Values);

        row++;
    }

    // ✅ Convert to a downloadable file
    var stream = new MemoryStream();
    package.SaveAs(stream);
    stream.Position = 0;
    var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    var fileName = "EmployeeUploadErrors.xlsx";

    return File(stream, contentType, fileName);
}
[HttpGet("by-canteen-role/{canteenId}")]
public async Task<IActionResult> GetEmployeesByCanteenRole(string canteenId)
{
    var employees = await _employeeService.GetEmployeesByCanteenIdAsync(canteenId);
    return Ok(employees);
}




    }}