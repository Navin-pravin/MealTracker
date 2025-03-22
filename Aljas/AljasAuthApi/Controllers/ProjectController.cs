using Microsoft.AspNetCore.Mvc;
using ProjectHierarchyApi.Models;
using ProjectHierarchyApi.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

using MongoDB.Bson;

namespace ProjectHierarchyApi.Controllers
{
    [Route("api/projects")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly ProjectService _projectService;

        public ProjectController(ProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpGet ("project-summary")]
        public async Task<ActionResult<List<Project>>> GetAllProjects() =>
            await _projectService.GetAllProjectsAsync();

        [HttpPost("add-project")]
        public async Task<IActionResult> CreateProject(Project project)
        {
            await _projectService.CreateProjectAsync(project);
            return Ok();
        }

        [HttpPut("update-project/{id}")]
public async Task<IActionResult> UpdateProject(string id, [FromBody] Project updatedProject)
{
    // Validate ID format
    if (string.IsNullOrEmpty(id) || !ObjectId.TryParse(id, out _))
        return BadRequest(new { message = "Invalid project ID format." });

    // Perform update
    bool success = await _projectService.UpdateProjectAsync(id, updatedProject);

    if (!success)
        return NotFound(new { message = "Project not found or update failed." });

    return Ok(new { message = "Project updated successfully." });
}

[HttpDelete("delete-project/{id}")]
public async Task<IActionResult> DeleteProject(string id)
{
    if (!ObjectId.TryParse(id, out _))
        return BadRequest(new { message = "Invalid Project ID format." });

    var success = await _projectService.DeleteProjectAsync(id);
    if (!success) return NotFound(new { message = "Project not found" });

    return Ok(new { message = "Project deleted successfully" });
}

    }
}
