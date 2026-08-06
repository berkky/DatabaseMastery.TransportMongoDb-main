using DatabaseMastery.TransportMongoDb.Services.AboutServices;
using Microsoft.AspNetCore.Mvc;
namespace DatabaseMastery.TransportMongoDb.ViewComponents.DefaultComponents
{
    public class _DefaultAboutComponentPartial : ViewComponent
    {
        private readonly IAboutService _AboutService;

        public _DefaultAboutComponentPartial(IAboutService AboutService)
        {
            _AboutService = AboutService;
        }

        public async Task<IViewComponentResult > InvokeAsync()
        {
            var values = await _AboutService.GetAllAboutsAsync();
            return View(values);
        }
    }
}
