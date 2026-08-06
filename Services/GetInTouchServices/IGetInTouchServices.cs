using DatabaseMastery.TransportMongoDb.Dtos.GetInTouchDto;

namespace DatabaseMastery.TransportMongoDb.Services.GetInTouchServices
{
    public interface IGetInTouchServices
    {
        Task<List<ResultGetInTouchDto>> GetAllGetInTouchesAsync();

        Task CreateGetInTouchAsync(CreateGetInTouchDto createGetInTouchDto);

        Task UpdateGetInTouchAsync(UpdateGetInTouchDto updateGetInTouchDto);

        Task<GetGetInTouchByIdDto> GetGetInTouchByIdAsync(string id);

        Task DeleteGetInTouchAsync(string id);
    }
}