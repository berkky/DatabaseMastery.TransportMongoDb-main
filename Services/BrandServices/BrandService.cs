using DatabaseMastery.TransportMongoDb.Dtos.BrandDtos;
using DatabaseMastery.TransportMongoDb.Entities;
using DatabaseMastery.TransportMongoDb.Mapping.Content;
using DatabaseMastery.TransportMongoDb.Settings;
using MongoDB.Driver;

namespace DatabaseMastery.TransportMongoDb.Services.BrandServices
{
    public class BrandService : IBrandService
    {
        private readonly IMongoCollection<Brand> _brandCollection;

        public BrandService(IDatabaseSettings databaseSettings)
        {
            var client = new MongoClient(databaseSettings.ConnectionString);
            var database = client.GetDatabase(databaseSettings.DatabaseName);

            _brandCollection = database.GetCollection<Brand>(
                databaseSettings.BrandCollectionName);
        }

        public async Task CreateBrandAsync(CreateBrandDto createBrandDto)
        {
            var value = BrandMapper.ToEntity(createBrandDto);
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

            return BrandMapper.ToResultList(values)!;
        }

        public async Task<GetBrandIdDto> GetBrandByIdAsync(string id)
        {
            var value = await _brandCollection
                .Find(x => x.BrandId == id)
                .FirstOrDefaultAsync();

            return BrandMapper.ToGetById(value)!;
        }

        public async Task UpdateBrandAsync(UpdateBrandDto updateBrandDto)
        {
            var value = BrandMapper.ToEntity(updateBrandDto);

            await _brandCollection.FindOneAndReplaceAsync(
                x => x.BrandId == updateBrandDto.BrandId,
                value);
        }
    }
}
