using DatabaseMastery.TransportMongoDb.Dtos.ProjectDtos;
using DatabaseMastery.TransportMongoDb.Services.ProjectServices;
using Microsoft.AspNetCore.Mvc;

namespace DatabaseMastery.TransportMongoDb.Controllers
{
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
        public IActionResult CreateProject()
        {
            return View("~/Views/AdminLayout/CreateProject.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> CreateProject(
            CreateProjectDto createProjectDto)
        {
            await _projectService.CreateProjectAsync(createProjectDto);
            return RedirectToAction(nameof(ProjectList));
        }

        public async Task<IActionResult> DeleteProject(string id)
        {
            await _projectService.DeleteProjectAsync(id);
            return RedirectToAction(nameof(ProjectList));
        }

        [HttpGet]
        public async Task<IActionResult> UpdateProject(string id)
        {
            var values = await _projectService.GetProjectByIdAsync(id);
            return View("~/Views/AdminLayout/UpdateProject.cshtml", values);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProject(
            UpdateProjectDto updateProjectDto)
        {
            await _projectService.UpdateProjectAsync(updateProjectDto);
            return RedirectToAction(nameof(ProjectList));
        }
    }
}
