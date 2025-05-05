using AljasAuthApi.Models;
using AljasAuthApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace AljasAuthApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly ReportService _reportService;

        public ReportController(ReportService reportService)
        {
            _reportService = reportService;
        }

        // Generate a report based on the provided request
        [HttpPost("generate")]
        public async Task<IActionResult> GenerateReport([FromBody] ReportRequest request)
        {
            var report = await _reportService.GenerateReport(request);
            return Ok(report);
        }
    } // <== This is the correct closing for the class
}
