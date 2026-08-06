using DatabaseMastery.TransportMongoDb.Services.ProjectServices;
using Microsoft.AspNetCore.Mvc;

namespace DatabaseMastery.TransportMongoDb.ViewComponents.DefaultComponents
{
    public class _DefaultWhatWeHaveDoneComponentPartial : ViewComponent
    {
        private readonly IProjectService _projectService;

        public _DefaultWhatWeHaveDoneComponentPartial(
            IProjectService projectService)
        {
            _projectService = projectService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var values = await _projectService.GetAllProjectsAsync();
            var activeProjects = values
                .Where(project => project.Status)
                .ToList();

            return View(activeProjects);
        }
    }
}
