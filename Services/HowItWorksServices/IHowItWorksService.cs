using DatabaseMastery.TransportMongoDb.Dtos.HowItWorksDto;

namespace DatabaseMastery.TransportMongoDb.Services.HowItWorksServices
{
    public interface IHowItWorksService
    {
        Task<List<ResultHowItWorksDto>> GetAllHowItWorksAsync();

        Task CreateHowItWorksAsync(CreateHowItWorksDto createHowItWorksDto);

        Task UpdateHowItWorksAsync(UpdateHowItWorksDto updateHowItWorksDto);

        Task<GetHowItWorksByIdDto> GetHowItWorksByIdAsync(string id);

        Task DeleteHowItWorksAsync(string id);
    }
}
