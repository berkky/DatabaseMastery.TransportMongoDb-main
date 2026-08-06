using DatabaseMastery.TransportMongoDb.Dtos.ProjectDtos;
using DatabaseMastery.TransportMongoDb.Security;
using DatabaseMastery.TransportMongoDb.Services.ProjectServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace DatabaseMastery.TransportMongoDb.Controllers
{
    [Authorize(Roles = AdminRoles.AllAdminRoles)]
    public class ProjectController : Controller
    {
        private readonly IProjectService _projectService;

        public ProjectController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        public async Task<IActionResult> ProjectList()
        {
            var values = await _projectService.GetAllProjectsAsync();
            return View("~/Views/AdminLayout/ProjectList.cshtml", values);
        }

        [HttpGet]
        [Authorize(Roles = AdminRoles.AdminsOnly)]
        public IActionResult CreateProject()
        {
            return View("~/Views/AdminLayout/CreateProject.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AdminRoles.AdminsOnly)]
        public async Task<IActionResult> CreateProject(
            CreateProjectDto createProjectDto)
        {
            if (!ModelState.IsValid)
            {
                return View(
                    "~/Views/AdminLayout/CreateProject.cshtml",
                    createProjectDto);
            }

            await _projectService.CreateProjectAsync(createProjectDto);
            return RedirectToAction(nameof(ProjectList));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AdminRoles.AdminsOnly)]
        public async Task<IActionResult> DeleteProject(string id)
        {
            await _projectService.DeleteProjectAsync(id);
            return RedirectToAction(nameof(ProjectList));
        }

        [HttpGet]
        [Authorize(Roles = AdminRoles.AdminsOnly)]
        public async Task<IActionResult> UpdateProject(string id)
        {
            if (!ObjectId.TryParse(id, out _))
            {
                return NotFound();
            }

            var values = await _projectService.GetProjectByIdAsync(id);
            if (values is null || string.IsNullOrWhiteSpace(values.ProjectId))
            {
                return NotFound();
            }

            var updateDto = new UpdateProjectDto
            {
                ProjectId = values.ProjectId,
                Title = values.Title,
                Description = values.Description,
                ImageUrl = values.ImageUrl,
                Status = values.Status
            };

            return View("~/Views/AdminLayout/UpdateProject.cshtml", updateDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AdminRoles.AdminsOnly)]
        public async Task<IActionResult> UpdateProject(
            string id,
            UpdateProjectDto updateProjectDto)
        {
            if (!ObjectId.TryParse(id, out _) ||
                !ObjectId.TryParse(updateProjectDto.ProjectId, out _) ||
                !string.Equals(
                    id,
                    updateProjectDto.ProjectId,
                    StringComparison.Ordinal))
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(
                    "~/Views/AdminLayout/UpdateProject.cshtml",
                    updateProjectDto);
            }

            await _projectService.UpdateProjectAsync(updateProjectDto);
            return RedirectToAction(nameof(ProjectList));
        }
    }
}
