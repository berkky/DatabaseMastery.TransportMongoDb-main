using DatabaseMastery.TransportMongoDb.Dtos.GetInTouchDto;
using DatabaseMastery.TransportMongoDb.Entities;
using DatabaseMastery.TransportMongoDb.Mapping.Content;
using DatabaseMastery.TransportMongoDb.Settings;
using MongoDB.Driver;

namespace DatabaseMastery.TransportMongoDb.Services.GetInTouchServices
{
    public class GetInTouchService : IGetInTouchServices
    {
        private readonly IMongoCollection<GetInTouchSection> _getInTouchCollection;

        public GetInTouchService(IDatabaseSettings databaseSettings)
        {
            var client = new MongoClient(databaseSettings.ConnectionString);
            var database = client.GetDatabase(databaseSettings.DatabaseName);

            _getInTouchCollection = database.GetCollection<GetInTouchSection>(
                databaseSettings.GetInTouchCollectionName);
        }

        public async Task CreateGetInTouchAsync(
            CreateGetInTouchDto createGetInTouchDto)
        {
            var value = GetInTouchMapper.ToEntity(createGetInTouchDto);

            await _getInTouchCollection.InsertOneAsync(value);
        }

        public async Task DeleteGetInTouchAsync(string id)
        {
            await _getInTouchCollection.DeleteOneAsync(
                x => x.GetInTouchSectionId == id);
        }

        public async Task<List<ResultGetInTouchDto>> GetAllGetInTouchesAsync()
        {
            var values = await _getInTouchCollection
                .Find(x => true)
                .ToListAsync();

            return GetInTouchMapper.ToResultList(values)!;
        }

        public async Task<GetGetInTouchByIdDto> GetGetInTouchByIdAsync(
            string id)
        {
            var value = await _getInTouchCollection
                .Find(x => x.GetInTouchSectionId == id)
                .FirstOrDefaultAsync();

            return GetInTouchMapper.ToGetById(value)!;
        }

        public async Task UpdateGetInTouchAsync(
            UpdateGetInTouchDto updateGetInTouchDto)
        {
            var value = GetInTouchMapper.ToEntity(updateGetInTouchDto);

            await _getInTouchCollection.FindOneAndReplaceAsync(
                x => x.GetInTouchSectionId == updateGetInTouchDto.GetInTouchSectionId,
                value);
        }
    }
}
