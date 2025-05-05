using AljasAuthApi.Models;
using AljasAuthApi.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace AljasAuthApi.Controllers
{
    [ApiController]
    [Route("api/consolidated-reports")]
    public class ConsolidatedReportController : ControllerBase
    {
        private readonly ConsolidatedReportService _consolidatedReportService;

        public ConsolidatedReportController(ConsolidatedReportService consolidatedReportService)
        {
            _consolidatedReportService = consolidatedReportService;
        }

        // POST: api/consolidated-reports
        [HttpPost]
        public async Task<IActionResult> GenerateReport([FromBody] ConsolidatedReportRequest request)
        {
            try
            {
                var result = await _consolidatedReportService.GenerateConsolidatedReport(request);

                if (result == null || result.IndividualReports == null || !result.IndividualReports.Any())
                {
                    return Ok(new
                    {
                        Data = new ConsolidatedReportWithSummary
                        {
                            IndividualReports = new List<ConsolidatedReportResponse>(),
                            Summary = result?.Summary ?? new ReportSummary
                            {
                                CanteenName = "All",
                                StartDate = request.StartDate,
                                EndDate = request.EndDate
                            }
                        },
                        Message = "No data found for the given criteria."
                    });
                }

                return Ok(new
                {
                    Data = result,
                    Message = "Report generated successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Error generating report: {ex.Message}" });
            }
        }
    } // Correct end of class
}     // Correct end of namespace
