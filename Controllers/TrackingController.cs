using DatabaseMastery.TransportMongoDb.Services.ShipmentServices;
using Microsoft.AspNetCore.Mvc;

namespace DatabaseMastery.TransportMongoDb.Controllers
{
    public class TrackingController : Controller
    {
        private readonly IShipmentService _shipmentService;

        public TrackingController(IShipmentService shipmentService)
        {
            _shipmentService = shipmentService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? trackingNumber)
        {
            var hasSearched = trackingNumber is not null ||
                ModelState.ContainsKey(nameof(trackingNumber));

            ViewBag.HasSearched = hasSearched;
            ViewBag.SearchedNumber = trackingNumber ?? string.Empty;

            if (!hasSearched)
            {
                return View();
            }

            if (string.IsNullOrWhiteSpace(trackingNumber))
            {
                ViewBag.ValidationMessage = "Lütfen takip numaranızı girin.";
                return View();
            }

            var trackingResult = await _shipmentService
                .GetPublicTrackingByTrackingNumberAsync(trackingNumber.Trim());

            return View(trackingResult);
        }
    }
}
