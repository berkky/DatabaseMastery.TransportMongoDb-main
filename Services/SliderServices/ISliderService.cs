using DatabaseMastery.TransportMongoDb.Dtos.SliderDto;

namespace DatabaseMastery.TransportMongoDb.Services.SliderServices
{
    public interface ISliderService
    {
        Task<List<ResultSliderDto>> GetAllSlidersAsync();

        Task CreateSliderAsync(CreateSliderDto createSliderDto);

        Task UpdateSliderAsync(UpdateSliderDto updateSliderDto);

        Task<GetSliderByIdDto> GetSliderByIdAsync(string id);

        Task DeleteSliderAsync(string id);
    }

}
