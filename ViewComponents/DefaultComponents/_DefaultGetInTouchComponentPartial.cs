using DatabaseMastery.TransportMongoDb.Services.GetInTouchServices;
using Microsoft.AspNetCore.Mvc;

namespace DatabaseMastery.TransportMongoDb.ViewComponents.DefaultComponents
{
    public class _DefaultGetInTouchComponentPartial : ViewComponent
    {
        private readonly IGetInTouchServices _getInTouchService;

        public _DefaultGetInTouchComponentPartial(
            IGetInTouchServices getInTouchService)
        {
            _getInTouchService = getInTouchService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var values = await _getInTouchService.GetAllGetInTouchesAsync();
            var value = values.FirstOrDefault(item => item.Status);

            return View(value);
        }
    }
}
