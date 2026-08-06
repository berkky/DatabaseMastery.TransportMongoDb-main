using AutoMapper;
using DatabaseMastery.TransportMongoDb.Dtos.ProjectDtos;
using DatabaseMastery.TransportMongoDb.Entities;
using DatabaseMastery.TransportMongoDb.Settings;
using MongoDB.Driver;

namespace DatabaseMastery.TransportMongoDb.Services.ProjectServices
{
    public class ProjectService : IProjectService
    {
        private readonly IMongoCollection<Project> _projectCollection;
        private readonly IMapper _mapper;

        public ProjectService(IMapper mapper, IDatabaseSettings databaseSettings)
        {
            var client = new MongoClient(databaseSettings.ConnectionString);
            var database = client.GetDatabase(databaseSettings.DatabaseName);

            _projectCollection = database.GetCollection<Project>(
                databaseSettings.ProjectCollectionName);

            _mapper = mapper;
        }

        public async Task CreateProjectAsync(CreateProjectDto createProjectDto)
        {
            var value = _mapper.Map<Project>(createProjectDto);
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

            return _mapper.Map<List<ResultProjectDto>>(values);
        }

        public async Task<GetProjectByIdDto> GetProjectByIdAsync(string id)
        {
            var value = await _projectCollection
                .Find(x => x.ProjectId == id)
                .FirstOrDefaultAsync();

            return _mapper.Map<GetProjectByIdDto>(value);
        }

        public async Task UpdateProjectAsync(UpdateProjectDto updateProjectDto)
        {
            var value = _mapper.Map<Project>(updateProjectDto);

            await _projectCollection.FindOneAndReplaceAsync(
                x => x.ProjectId == updateProjectDto.ProjectId,
                value);
        }
    }
}
