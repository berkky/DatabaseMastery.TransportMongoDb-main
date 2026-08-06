using DatabaseMastery.TransportMongoDb.Dtos.AboutDto;
using DatabaseMastery.TransportMongoDb.Entities;
using DatabaseMastery.TransportMongoDb.Mapping.Content;
using DatabaseMastery.TransportMongoDb.Settings;
using MongoDB.Driver;

namespace DatabaseMastery.TransportMongoDb.Services.AboutServices
{
    public class AboutService: IAboutService
    {
        private readonly IMongoCollection<About> _AboutCollection;

        public AboutService(IDatabaseSettings databaseSettings)
        {
            var client = new MongoClient(databaseSettings.ConnectionString);
            var database = client.GetDatabase(databaseSettings.DatabaseName);

            _AboutCollection = database.GetCollection<About>(
                databaseSettings.AboutCollectionName);
        }

        public async Task CreateAboutAsync(CreateAboutDto createAboutDto)
        {
            var value = AboutMapper.ToEntity(createAboutDto);
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

            return AboutMapper.ToResultList(values)!;
        }

        public async Task<GetAboutByIdDto> GetAboutByIdAsync(string id)
        {
            var value = await _AboutCollection
                .Find(x => x.AboutId == id)
                .FirstOrDefaultAsync();

            return AboutMapper.ToGetById(value)!;
        }

        public async Task UpdateAboutAsync(UpdateAboutDto updateAboutDto)
        {
            var value = AboutMapper.ToEntity(updateAboutDto);

            await _AboutCollection.FindOneAndReplaceAsync(
                x => x.AboutId == updateAboutDto.AboutId,
                value);
        }
    }
}
