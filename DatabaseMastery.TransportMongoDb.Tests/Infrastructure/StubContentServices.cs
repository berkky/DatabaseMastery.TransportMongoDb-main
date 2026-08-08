using DatabaseMastery.TransportMongoDb.Dtos.AboutDto;
using DatabaseMastery.TransportMongoDb.Dtos.BrandDtos;
using DatabaseMastery.TransportMongoDb.Dtos.GetInTouchDto;
using DatabaseMastery.TransportMongoDb.Dtos.HowItWorksDto;
using DatabaseMastery.TransportMongoDb.Dtos.OfferDto;
using DatabaseMastery.TransportMongoDb.Dtos.ProjectDtos;
using DatabaseMastery.TransportMongoDb.Dtos.SliderDto;
using DatabaseMastery.TransportMongoDb.Dtos.TestimonialDtos;
using DatabaseMastery.TransportMongoDb.Services.AboutServices;
using DatabaseMastery.TransportMongoDb.Services.BrandServices;
using DatabaseMastery.TransportMongoDb.Services.GetInTouchServices;
using DatabaseMastery.TransportMongoDb.Services.HowItWorksServices;
using DatabaseMastery.TransportMongoDb.Services.OfferServices;
using DatabaseMastery.TransportMongoDb.Services.ProjectServices;
using DatabaseMastery.TransportMongoDb.Services.SliderServices;
using DatabaseMastery.TransportMongoDb.Services.TestimonialServices;

namespace DatabaseMastery.TransportMongoDb.Tests.Infrastructure;

internal sealed class StubSliderService : ISliderService
{
    public Task<List<ResultSliderDto>> GetAllSlidersAsync() => Task.FromResult(new List<ResultSliderDto>());
    public Task CreateSliderAsync(CreateSliderDto createSliderDto) => Task.CompletedTask;
    public Task UpdateSliderAsync(UpdateSliderDto updateSliderDto) => Task.CompletedTask;
    public Task<GetSliderByIdDto> GetSliderByIdAsync(string id) => Task.FromResult(new GetSliderByIdDto());
    public Task DeleteSliderAsync(string id) => Task.CompletedTask;
}

internal sealed class StubBrandService : IBrandService
{
    public Task<List<ResultBrandDto>> GetAllBrandsAsync() => Task.FromResult(new List<ResultBrandDto>());
    public Task CreateBrandAsync(CreateBrandDto createBrandDto) => Task.CompletedTask;
    public Task UpdateBrandAsync(UpdateBrandDto updateBrandDto) => Task.CompletedTask;
    public Task<GetBrandIdDto> GetBrandByIdAsync(string id) => Task.FromResult(new GetBrandIdDto());
    public Task DeleteBrandAsync(string id) => Task.CompletedTask;
}

internal sealed class StubOfferService : IOfferService
{
    public Task<List<ResultOfferDto>> GetAllOffersAsync() => Task.FromResult(new List<ResultOfferDto>());
    public Task CreateOfferAsync(CreateOfferDto createOfferDto) => Task.CompletedTask;
    public Task UpdateOfferAsync(UpdateOfferDto updateOfferDto) => Task.CompletedTask;
    public Task<GetOfferByIdDto> GetOfferByIdAsync(string id) => Task.FromResult(new GetOfferByIdDto());
    public Task DeleteOfferAsync(string id) => Task.CompletedTask;
}

internal sealed class StubAboutService : IAboutService
{
    public Task<List<ResultAboutDto>> GetAllAboutsAsync() => Task.FromResult(new List<ResultAboutDto>());
    public Task CreateAboutAsync(CreateAboutDto createAboutDto) => Task.CompletedTask;
    public Task UpdateAboutAsync(UpdateAboutDto updateAboutDto) => Task.CompletedTask;
    public Task<GetAboutByIdDto> GetAboutByIdAsync(string id) => Task.FromResult(new GetAboutByIdDto());
    public Task DeleteAboutAsync(string id) => Task.CompletedTask;
}

internal sealed class StubGetInTouchService : IGetInTouchServices
{
    public Task<List<ResultGetInTouchDto>> GetAllGetInTouchesAsync() =>
        Task.FromResult(new List<ResultGetInTouchDto>());
    public Task CreateGetInTouchAsync(CreateGetInTouchDto createGetInTouchDto) => Task.CompletedTask;
    public Task UpdateGetInTouchAsync(UpdateGetInTouchDto updateGetInTouchDto) => Task.CompletedTask;
    public Task<GetGetInTouchByIdDto> GetGetInTouchByIdAsync(string id) =>
        Task.FromResult(new GetGetInTouchByIdDto());
    public Task DeleteGetInTouchAsync(string id) => Task.CompletedTask;
}

internal sealed class StubHowItWorksService : IHowItWorksService
{
    public Task<List<ResultHowItWorksDto>> GetAllHowItWorksAsync() =>
        Task.FromResult(new List<ResultHowItWorksDto>());
    public Task CreateHowItWorksAsync(CreateHowItWorksDto createHowItWorksDto) => Task.CompletedTask;
    public Task UpdateHowItWorksAsync(UpdateHowItWorksDto updateHowItWorksDto) => Task.CompletedTask;
    public Task<GetHowItWorksByIdDto> GetHowItWorksByIdAsync(string id) =>
        Task.FromResult(new GetHowItWorksByIdDto());
    public Task DeleteHowItWorksAsync(string id) => Task.CompletedTask;
}

internal sealed class StubTestimonialService : ITestimonialService
{
    public Task<List<ResultTestimonialDto>> GetAllTestimonialAsync() =>
        Task.FromResult(new List<ResultTestimonialDto>());
    public Task CreateTestimonialAsync(CreateTestimonialDto createTestimonialDto) => Task.CompletedTask;
    public Task UpdateTestimonialAsync(UpdateTestimonialDto updateTestimonialDto) => Task.CompletedTask;
    public Task<GetTestimonialByIdDto> GetTestimonialByIdAsync(string id) =>
        Task.FromResult(new GetTestimonialByIdDto());
    public Task DeleteTestimonialAsync(string id) => Task.CompletedTask;
}

internal sealed class StubProjectService : IProjectService
{
    public Task<List<ResultProjectDto>> GetAllProjectsAsync() => Task.FromResult(new List<ResultProjectDto>());
    public Task CreateProjectAsync(CreateProjectDto createProjectDto) => Task.CompletedTask;
    public Task UpdateProjectAsync(UpdateProjectDto updateProjectDto) => Task.CompletedTask;
    public Task<GetProjectByIdDto> GetProjectByIdAsync(string id) => Task.FromResult(new GetProjectByIdDto());
    public Task DeleteProjectAsync(string id) => Task.CompletedTask;
}
