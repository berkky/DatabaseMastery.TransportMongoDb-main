using AutoMapper;
using DatabaseMastery.TransportMongoDb.Dtos.HowItWorksDto;
using DatabaseMastery.TransportMongoDb.Entities;
using DatabaseMastery.TransportMongoDb.Settings;
using MongoDB.Driver;

namespace DatabaseMastery.TransportMongoDb.Services.HowItWorksServices
{
    public class HowItWorksService : IHowItWorksService
    {
        private readonly IMongoCollection<HowItWorks> _howItWorksCollection;
        private readonly IMapper _mapper;

        public HowItWorksService(
            IMapper mapper,
            IDatabaseSettings databaseSettings)
        {
            var client = new MongoClient(databaseSettings.ConnectionString);
            var database = client.GetDatabase(databaseSettings.DatabaseName);

            _howItWorksCollection = database.GetCollection<HowItWorks>(
                databaseSettings.HowItWorksCollectionName);

            _mapper = mapper;
        }

        public async Task CreateHowItWorksAsync(CreateHowItWorksDto createHowItWorksDto)
        {
            var value = _mapper.Map<HowItWorks>(createHowItWorksDto);
            await _howItWorksCollection.InsertOneAsync(value);
        }

        public async Task DeleteHowItWorksAsync(string id)
        {
            await _howItWorksCollection.DeleteOneAsync(x => x.HowItWorksId == id);
        }

        public async Task<List<ResultHowItWorksDto>> GetAllHowItWorksAsync()
        {
            var values = await _howItWorksCollection
                .Find(x => true)
                .ToListAsync();

            return _mapper.Map<List<ResultHowItWorksDto>>(values);
        }

        public async Task<GetHowItWorksByIdDto> GetHowItWorksByIdAsync(string id)
        {
            var value = await _howItWorksCollection
                .Find(x => x.HowItWorksId == id)
                .FirstOrDefaultAsync();

            return _mapper.Map<GetHowItWorksByIdDto>(value);
        }

        public async Task UpdateHowItWorksAsync(UpdateHowItWorksDto updateHowItWorksDto)
        {
            var value = _mapper.Map<HowItWorks>(updateHowItWorksDto);

            await _howItWorksCollection.FindOneAndReplaceAsync(
                x => x.HowItWorksId == updateHowItWorksDto.HowItWorksId,
                value);
        }
    }
}
