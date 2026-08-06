using DatabaseMastery.TransportMongoDb.Dtos.BrandDtos;
namespace DatabaseMastery.TransportMongoDb.Services.BrandServices
{
    public interface IBrandService
    {
        Task<List<ResultBrandDto>> GetAllBrandsAsync();

        Task CreateBrandAsync(CreateBrandDto createBrandDto);

        Task UpdateBrandAsync(UpdateBrandDto updateBrandDto);

        Task<GetBrandIdDto> GetBrandByIdAsync(string id);

        Task DeleteBrandAsync(string id);
    }
}
