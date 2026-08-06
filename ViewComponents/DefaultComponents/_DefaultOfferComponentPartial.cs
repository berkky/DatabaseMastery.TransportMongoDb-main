using DatabaseMastery.TransportMongoDb.Services.OfferServices;
using Microsoft.AspNetCore.Mvc;

namespace DatabaseMastery.TransportMongoDb.ViewComponents.DefaultComponents
{
    public class _DefaultOfferComponentPartial : ViewComponent
    {
        private readonly IOfferService _offerService;

        public _DefaultOfferComponentPartial(IOfferService offerService)
        {
            _offerService = offerService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var values = await _offerService.GetAllOffersAsync();
            return View(values);
        }
    }
}
