using DatabaseMastery.TransportMongoDb.Dtos.ProjectDtos;
using DatabaseMastery.TransportMongoDb.Security;
using DatabaseMastery.TransportMongoDb.Services.ProjectServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        [Authorize(Roles = AdminRoles.AdminsOnly)]
        public async Task<IActionResult> CreateProject(
            CreateProjectDto createProjectDto)
        {
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
            var values = await _projectService.GetProjectByIdAsync(id);
            return View("~/Views/AdminLayout/UpdateProject.cshtml", values);
        }

        [HttpPost]
        [Authorize(Roles = AdminRoles.AdminsOnly)]
        public async Task<IActionResult> UpdateProject(
            UpdateProjectDto updateProjectDto)
        {
            await _projectService.UpdateProjectAsync(updateProjectDto);
            return RedirectToAction(nameof(ProjectList));
        }
    }
}
