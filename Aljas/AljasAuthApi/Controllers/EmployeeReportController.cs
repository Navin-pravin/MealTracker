using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AljasAuthApi.Services;
using AljasAuthApi.Models;

namespace AljasAuthApi.Controllers
{
    [Route("api/employee-reports")]
    [ApiController]
    public class EmployeeReportController : ControllerBase
    {
        private readonly EmployeeReportService _employeeReportService;

        public EmployeeReportController(EmployeeReportService employeeReportService)
        {
            _employeeReportService = employeeReportService;
        }

        // ✅ Create Employee Report API
        [HttpPost("create")]
        public async Task<IActionResult> CreateEmployeeReport([FromBody] EmployeeReport report)
        {
            if (report == null)
                return BadRequest(new { message = "Invalid report data." });

            await _employeeReportService.CreateEmployeeReportAsync(report);
            return Ok(new { message = "Employee report created successfully." });
        }

        // ✅ Employee Report Summary API
        [HttpGet("summary")]
        public async Task<IActionResult> GetEmployeeReportSummary()
        {
            var reports = await _employeeReportService.GetEmployeeReportSummaryAsync();
            return Ok(reports);
        }

        // ✅ Download Employee Report as Excel
        [HttpGet("download-excel")]
        public async Task<IActionResult> DownloadEmployeeReportExcel()
        {
            var fileData = await _employeeReportService.GenerateExcelReportAsync();
            return File(fileData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "EmployeeReports.xlsx");
        }
    }
}
