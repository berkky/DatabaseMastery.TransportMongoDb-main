using DatabaseMastery.TransportMongoDb.Dtos.ProjectDtos;
using DatabaseMastery.TransportMongoDb.Entities;
using DatabaseMastery.TransportMongoDb.Mapping.Content;
using DatabaseMastery.TransportMongoDb.Settings;
using MongoDB.Driver;

namespace DatabaseMastery.TransportMongoDb.Services.ProjectServices
{
    public class ProjectService : IProjectService
    {
        private readonly IMongoCollection<Project> _projectCollection;

        public ProjectService(IDatabaseSettings databaseSettings)
        {
            var client = new MongoClient(databaseSettings.ConnectionString);
            var database = client.GetDatabase(databaseSettings.DatabaseName);

            _projectCollection = database.GetCollection<Project>(
                databaseSettings.ProjectCollectionName);
        }

        public async Task CreateProjectAsync(CreateProjectDto createProjectDto)
        {
            var value = ProjectMapper.ToEntity(createProjectDto);
            await _projectCollection.InsertOneAsync(value);
        }

        public async Task DeleteProjectAsync(string id)
        {
            await _projectCollection.DeleteOneAsync(x => x.ProjectId == id);
        }

        public async Task<List<ResultProjectDto>> GetAllProjectsAsync()
        {
            var values = await _projectCollection
                .Find(x => true)
                .ToListAsync();

            return ProjectMapper.ToResultList(values)!;
        }

        public async Task<GetProjectByIdDto> GetProjectByIdAsync(string id)
        {
            var value = await _projectCollection
                .Find(x => x.ProjectId == id)
                .FirstOrDefaultAsync();

            return ProjectMapper.ToGetById(value)!;
        }

        public async Task UpdateProjectAsync(UpdateProjectDto updateProjectDto)
        {
            var value = ProjectMapper.ToEntity(updateProjectDto);

            await _projectCollection.FindOneAndReplaceAsync(
                x => x.ProjectId == updateProjectDto.ProjectId,
                value);
        }
    }
}
