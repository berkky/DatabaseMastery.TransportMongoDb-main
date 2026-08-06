using AutoMapper;
using DatabaseMastery.TransportMongoDb.Dtos.BrandDtos;
using DatabaseMastery.TransportMongoDb.Entities;
using DatabaseMastery.TransportMongoDb.Settings;
using MongoDB.Driver;

namespace DatabaseMastery.TransportMongoDb.Services.BrandServices
{
    public class BrandService : IBrandService
    {
        private readonly IMongoCollection<Brand> _brandCollection;
        private readonly IMapper _mapper;

        public BrandService(
            IMapper mapper,
            IDatabaseSettings databaseSettings)
        {
            var client = new MongoClient(databaseSettings.ConnectionString);
            var database = client.GetDatabase(databaseSettings.DatabaseName);

            _brandCollection = database.GetCollection<Brand>(
                databaseSettings.BrandCollectionName);

            _mapper = mapper;
        }

        public async Task CreateBrandAsync(CreateBrandDto createBrandDto)
        {
            var value = _mapper.Map<Brand>(createBrandDto);
            await _brandCollection.InsertOneAsync(value);
        }

        public async Task DeleteBrandAsync(string id)
        {
            await _brandCollection.DeleteOneAsync(x => x.BrandId == id);
        }

        public async Task<List<ResultBrandDto>> GetAllBrandsAsync()
        {
            var values = await _brandCollection
                .Find(x => true)
                .ToListAsync();

            return _mapper.Map<List<ResultBrandDto>>(values);
        }

        public async Task<GetBrandIdDto> GetBrandByIdAsync(string id)
        {
            var value = await _brandCollection
                .Find(x => x.BrandId == id)
                .FirstOrDefaultAsync();

            return _mapper.Map<GetBrandIdDto>(value);
        }

        public async Task UpdateBrandAsync(UpdateBrandDto updateBrandDto)
        {
            var value = _mapper.Map<Brand>(updateBrandDto);

            await _brandCollection.FindOneAndReplaceAsync(
                x => x.BrandId == updateBrandDto.BrandId,
                value);
        }
    }
}