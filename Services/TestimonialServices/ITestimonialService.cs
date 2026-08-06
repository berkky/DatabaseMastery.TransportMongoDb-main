using DatabaseMastery.TransportMongoDb.Dtos.TestimonialDtos;

namespace DatabaseMastery.TransportMongoDb.Services.TestimonialServices
{
    public interface ITestimonialService
    {
        Task<List<ResultTestimonialDto>> GetAllTestimonialAsync();
        Task CreateTestimonialAsync(CreateTestimonialDto createTestimonialDto);
        Task UpdateTestimonialAsync(UpdateTestimonialDto updateTestimonialDto);
        Task<GetTestimonialByIdDto> GetTestimonialByIdAsync(string id);
        Task DeleteTestimonialAsync(string id);
    }
}
