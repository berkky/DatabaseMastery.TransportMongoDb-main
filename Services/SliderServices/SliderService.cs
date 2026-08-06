using DatabaseMastery.TransportMongoDb.Dtos.SliderDto;
using DatabaseMastery.TransportMongoDb.Entities;
using DatabaseMastery.TransportMongoDb.Mapping.Content;
using DatabaseMastery.TransportMongoDb.Settings;
using MongoDB.Driver;

namespace DatabaseMastery.TransportMongoDb.Services.SliderServices
{
    public class SliderService : ISliderService
    {
        private readonly IMongoCollection<Slider> _sliderCollection;

        public SliderService(IDatabaseSettings _databaseSettings)
        {
            var client=new MongoClient(_databaseSettings.ConnectionString);
            var database= client.GetDatabase(_databaseSettings.DatabaseName);
            _sliderCollection = database.GetCollection<Slider>(_databaseSettings.SliderCollectionName);
        }
        public async Task CreateSliderAsync(CreateSliderDto createSliderDto)
        {
            var value = SliderMapper.ToEntity(createSliderDto);
            await _sliderCollection.InsertOneAsync(value);
        }

        public async Task DeleteSliderAsync(string id)
        {
           await _sliderCollection.DeleteOneAsync(x => x.SliderId == id);
        }

        public async Task<List<ResultSliderDto>> GetAllSlidersAsync()
        {
            var values=await _sliderCollection.Find(x => true).ToListAsync();
            return SliderMapper.ToResultList(values)!;
        }

        public async Task<GetSliderByIdDto> GetSliderByIdAsync(string id)
        {
            var value = await _sliderCollection
                .Find(x => x.SliderId == id)
                .FirstOrDefaultAsync();

            return SliderMapper.ToGetById(value)!;
        }

        public async Task UpdateSliderAsync(UpdateSliderDto updateSliderDto)
        {
            var value = SliderMapper.ToEntity(updateSliderDto);
            await _sliderCollection.FindOneAndReplaceAsync(x => x.SliderId == updateSliderDto.SliderId, value);
        }
    }
}
