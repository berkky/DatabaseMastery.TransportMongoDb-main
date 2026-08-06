using DatabaseMastery.TransportMongoDb.Services.BrandServices;
using Microsoft.AspNetCore.Mvc;
namespace DatabaseMastery.TransportMongoDb.ViewComponents.DefaultComponents
{
    public class _DefaultBrandComponentPartial : ViewComponent

    {
        private readonly IBrandService _BrandService;

        public _DefaultBrandComponentPartial(IBrandService BrandService)
        {
            _BrandService = BrandService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var values = await _BrandService.GetAllBrandsAsync();
            return View(values);
        }
    }
}
