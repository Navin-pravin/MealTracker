using AljasAuthApi.Services;  // Add this line
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AljasAuthApi.Controllers
{
   [ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly DashboardService _dashboardService;

    public DashboardController(DashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpPost("dashboard-preview")]
    public async Task<IActionResult> CreateDashboardPreview([FromBody] DashboardPreviewRequest request)
    {
        var mealTypes = await _dashboardService.GetMealTypesByLocationAndCanteenAsync(request.LocationId, request.CanteenId);

        if (mealTypes == null || !mealTypes.Any())
            return NotFound("No meal types found for the provided location and canteen.");

        // Save dashboard
        var dashboard = new Dashboard
        {
            
            DashboardName = request.DashboardName,
            LocationId = request.LocationId,
            LocationName = request.LocationName,
            CanteenId = request.CanteenId,
            CanteenName = request.CanteenName,
            MealTypes = mealTypes
        };

        await _dashboardService.CreateDashboardAsync(dashboard);

        return Ok(new
        {
            Message = "Dashboard added successfully",
            Dashboard = new
            {
                dashboard.Id,
                dashboard.DashboardName,
                dashboard.LocationId,
                dashboard.LocationName,
                dashboard.CanteenId,
                dashboard.CanteenName,
                MealTypes = dashboard.MealTypes
            }
        });
    }

    [HttpGet("dashboard-summary")]
    public async Task<IActionResult> GetDashboardSummary()
    {
        var dashboards = await _dashboardService.GetAllDashboardsAsync();

        var summary = dashboards.Select(d => new
        {
            d.Id,
            d.DashboardName,
            d.LocationId,
            d.LocationName,
            d.CanteenId,
            d.CanteenName,
            MealTypes = d.MealTypes
        });

        return Ok(summary);
    }
    [HttpGet("dashboard-mealtype-summary")]
public async Task<IActionResult> GetDashboardMealCounts()
{
    var summary = await _dashboardService.GetDashboardMealCountsAsync();
    return Ok(summary);
}
[HttpDelete("{id}")]
public async Task<IActionResult> DeleteDashboard(string id)
{
    var success = await _dashboardService.DeleteDashboardByIdAsync(id);
    if (!success)
    {
        return NotFound(new { message = "Dashboard not found" });
    }

    return Ok(new { message = "Dashboard deleted successfully" });
}

}

}
