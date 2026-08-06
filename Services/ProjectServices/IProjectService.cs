using DatabaseMastery.TransportMongoDb.Dtos.ProjectDtos;

namespace DatabaseMastery.TransportMongoDb.Services.ProjectServices
{
    public interface IProjectService
    {
        Task<List<ResultProjectDto>> GetAllProjectsAsync();

        Task CreateProjectAsync(CreateProjectDto createProjectDto);

        Task UpdateProjectAsync(UpdateProjectDto updateProjectDto);

        Task<GetProjectByIdDto> GetProjectByIdAsync(string id);

        Task DeleteProjectAsync(string id);
    }
}
