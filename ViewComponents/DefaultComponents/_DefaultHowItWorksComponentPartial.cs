using Microsoft.AspNetCore.Mvc;
using DatabaseMastery.TransportMongoDb.Services.HowItWorksServices;

namespace DatabaseMastery.TransportMongoDb.ViewComponents.DefaultComponents
{
    public class _DefaultHowItWorksComponentPartial : ViewComponent
    {
        private readonly IHowItWorksService _howItWorksService;

        public _DefaultHowItWorksComponentPartial(IHowItWorksService howItWorksService)
        {
            _howItWorksService = howItWorksService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var values = await _howItWorksService.GetAllHowItWorksAsync();
            return View(values);
        }
    }
}
