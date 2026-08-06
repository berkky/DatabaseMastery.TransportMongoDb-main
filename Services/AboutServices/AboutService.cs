using AutoMapper;
using DatabaseMastery.TransportMongoDb.Dtos.AboutDto;
using DatabaseMastery.TransportMongoDb.Entities;
using DatabaseMastery.TransportMongoDb.Settings;
using MongoDB.Driver;

namespace DatabaseMastery.TransportMongoDb.Services.AboutServices
{
    public class AboutService: IAboutService
    {
        private readonly IMongoCollection<About> _AboutCollection;
        private readonly IMapper _mapper;

        public AboutService(
            IMapper mapper,
            IDatabaseSettings databaseSettings)
        {
            var client = new MongoClient(databaseSettings.ConnectionString);
            var database = client.GetDatabase(databaseSettings.DatabaseName);

            _AboutCollection = database.GetCollection<About>(
                databaseSettings.AboutCollectionName);

            _mapper = mapper;
        }

        public async Task CreateAboutAsync(CreateAboutDto createAboutDto)
        {
            var value = _mapper.Map<About>(createAboutDto);
            await _AboutCollection.InsertOneAsync(value);
        }

        public async Task DeleteAboutAsync(string id)
        {
            await _AboutCollection.DeleteOneAsync(x => x.AboutId == id);
        }

        public async Task<List<ResultAboutDto>> GetAllAboutsAsync()
        {
            var values = await _AboutCollection
                .Find(x => true)
                .ToListAsync();

            return _mapper.Map<List<ResultAboutDto>>(values);
        }

        public async Task<GetAboutByIdDto> GetAboutByIdAsync(string id)
        {
            var value = await _AboutCollection
                .Find(x => x.AboutId == id)
                .FirstOrDefaultAsync();

            return _mapper.Map<GetAboutByIdDto>(value);
        }

        public async Task UpdateAboutAsync(UpdateAboutDto updateAboutDto)
        {
            var value = _mapper.Map<About>(updateAboutDto);

            await _AboutCollection.FindOneAndReplaceAsync(
                x => x.AboutId == updateAboutDto.AboutId,
                value);
        }
    }
}
