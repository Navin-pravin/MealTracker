using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AljasAuthApi.Models;
using OfficeOpenXml;

namespace AljasAuthApi.Services
{
    public class EmployeeReportService
    {
        private readonly IMongoCollection<EmployeeReport> _employeeReports;

        public EmployeeReportService(MongoDbSettings dbSettings)
        {
            var client = new MongoClient(dbSettings.ConnectionString);
            var database = client.GetDatabase(dbSettings.DatabaseName);
            _employeeReports = database.GetCollection<EmployeeReport>("EmployeeReports");
        }

        // ✅ Create Employee Report
        public async Task CreateEmployeeReportAsync(EmployeeReport report)
        {
            await _employeeReports.InsertOneAsync(report);
        }

        // ✅ Get Employee Report Summary
        public async Task<List<EmployeeReport>> GetEmployeeReportSummaryAsync()
        {
            return await _employeeReports.Find(_ => true).ToListAsync();
        }

        // ✅ Generate Excel Report
        public async Task<byte[]> GenerateExcelReportAsync()
        {
            var reports = await GetEmployeeReportSummaryAsync();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Employee Reports");

                // ✅ Add Header
                worksheet.Cells[1, 1].Value = "Date";
                worksheet.Cells[1, 2].Value = "ID Number";
                worksheet.Cells[1, 3].Value = "First Name";
                worksheet.Cells[1, 4].Value = "Last Name";
                worksheet.Cells[1, 5].Value = "Company";
                worksheet.Cells[1, 6].Value = "Designation";
                worksheet.Cells[1, 7].Value = "Role Name";
                worksheet.Cells[1, 8].Value = "Location";
                worksheet.Cells[1, 9].Value = "Department";
                worksheet.Cells[1, 10].Value = "Car Badge Number";
                worksheet.Cells[1, 11].Value = "Device Name";
                worksheet.Cells[1, 12].Value = "Punching Time";
                worksheet.Cells[1, 13].Value = "Canteen";
                worksheet.Cells[1, 14].Value = "Meal Type";

                int row = 2;
                foreach (var report in reports)
                {
                    worksheet.Cells[row, 1].Value = report.Date;
                    worksheet.Cells[row, 2].Value = report.IdNumber;
                    worksheet.Cells[row, 3].Value = report.FirstName;
                    worksheet.Cells[row, 4].Value = report.LastName;
                    worksheet.Cells[row, 5].Value = report.Company;
                    worksheet.Cells[row, 6].Value = report.Designation;
                    worksheet.Cells[row, 7].Value = report.RoleName;
                    worksheet.Cells[row, 8].Value = report.Location;
                    worksheet.Cells[row, 9].Value = report.Department;
                    worksheet.Cells[row, 10].Value = report.CarBadgeNumber;
                    worksheet.Cells[row, 11].Value = report.DeviceName;
                    worksheet.Cells[row, 12].Value = report.PunchingTime;
                    worksheet.Cells[row, 13].Value = report.Canteen;
                    worksheet.Cells[row, 14].Value = report.MealType;
                    row++;
                }

                return package.GetAsByteArray();
            }
        }
    }
}
