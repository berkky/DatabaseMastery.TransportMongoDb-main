using DatabaseMastery.TransportMongoDb.Dtos.OfferDto;
using DatabaseMastery.TransportMongoDb.Services.OfferServices;
using Microsoft.AspNetCore.Mvc;

namespace DatabaseMastery.TransportMongoDb.Controllers
{
    public class OfferController : Controller
    {
        private readonly IOfferService _OfferService;

        public OfferController(IOfferService OfferService)
        {
            _OfferService = OfferService;
        }

        public async Task<IActionResult> Offerlist()
        {
            var values = await _OfferService.GetAllOffersAsync();
            return View("~/Views/AdminLayout/OfferList.cshtml", values);
        }

        [HttpGet]
        public IActionResult CreateOffer()
        {
            return View("~/Views/AdminLayout/CreateOffer.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> CreateOffer(CreateOfferDto createOfferDto)
        {
            await _OfferService.CreateOfferAsync(createOfferDto);
            return RedirectToAction(nameof(Offerlist));
        }

        public async Task<IActionResult> DeleteOffer(string id)
        {
            await _OfferService.DeleteOfferAsync(id);
            return RedirectToAction(nameof(Offerlist));
        }

        [HttpGet]
        public async Task<IActionResult> UpdateOffer(string id)
        {
            var values = await _OfferService.GetOfferByIdAsync(id);
            return View("~/Views/AdminLayout/UpdateOffer.cshtml", values);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOffer(UpdateOfferDto updateOfferDto)
        {
            await _OfferService.UpdateOfferAsync(updateOfferDto);
            return RedirectToAction(nameof(Offerlist));
        }
    }
}

