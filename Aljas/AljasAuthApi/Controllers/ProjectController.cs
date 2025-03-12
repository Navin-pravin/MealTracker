using Microsoft.AspNetCore.Mvc;
using ProjectHierarchyApi.Models;
using ProjectHierarchyApi.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        [HttpGet ("project summary")]
        public async Task<ActionResult<List<Project>>> GetAllProjects() =>
            await _projectService.GetAllProjectsAsync();

        [HttpPost("add project")]
        public async Task<IActionResult> CreateProject(Project project)
        {
            await _projectService.CreateProjectAsync(project);
            return Ok();
        }

        [HttpPut("{id} update project")]
        public async Task<IActionResult> UpdateProject(string id, Project updatedProject)
        {
            var success = await _projectService.UpdateProjectAsync(id, updatedProject);
            if (!success) return NotFound();
            return Ok();
        }

        [HttpDelete("{id} delete project")]
        public async Task<IActionResult> DeleteProject(string id)
        {
            var success = await _projectService.DeleteProjectAsync(id);
            if (!success) return NotFound();
            return Ok();
        }
    }
}
