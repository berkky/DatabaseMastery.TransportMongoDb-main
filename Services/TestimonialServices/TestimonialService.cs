using DatabaseMastery.TransportMongoDb.Dtos.TestimonialDtos;
using DatabaseMastery.TransportMongoDb.Entities;
using DatabaseMastery.TransportMongoDb.Mapping.Content;
using DatabaseMastery.TransportMongoDb.Settings;
using MongoDB.Driver;

namespace DatabaseMastery.TransportMongoDb.Services.TestimonialServices
{
    public class TestimonialService : ITestimonialService
    {
        private readonly IMongoCollection<Testimonial> _testimonialCollection;

        public TestimonialService(IDatabaseSettings databaseSettings)
        {
            var client = new MongoClient(databaseSettings.ConnectionString);
            var database = client.GetDatabase(databaseSettings.DatabaseName);
            _testimonialCollection = database.GetCollection<Testimonial>(
                databaseSettings.TestimonialCollectionName);
        }

        public async Task CreateTestimonialAsync(
            CreateTestimonialDto createTestimonialDto)
        {
            var value = TestimonialMapper.ToEntity(createTestimonialDto);
            await _testimonialCollection.InsertOneAsync(value);
        }

        public async Task DeleteTestimonialAsync(string id)
        {
            await _testimonialCollection.DeleteOneAsync(
                x => x.TestimonialId == id);
        }

        public async Task<List<ResultTestimonialDto>> GetAllTestimonialAsync()
        {
            var values = await _testimonialCollection.Find(x => true).ToListAsync();
            return TestimonialMapper.ToResultList(values)!;
        }

        public async Task<GetTestimonialByIdDto> GetTestimonialByIdAsync(string id)
        {
            var value = await _testimonialCollection
                .Find(x => x.TestimonialId == id)
                .FirstOrDefaultAsync();

            return TestimonialMapper.ToGetById(value)!;
        }

        public async Task UpdateTestimonialAsync(
            UpdateTestimonialDto updateTestimonialDto)
        {
            var value = TestimonialMapper.ToEntity(updateTestimonialDto);
            await _testimonialCollection.FindOneAndReplaceAsync(
                x => x.TestimonialId == updateTestimonialDto.TestimonialId,
                value);
        }
    }
}
