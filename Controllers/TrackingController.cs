using DatabaseMastery.TransportMongoDb.Services.ShipmentServices;
using Microsoft.AspNetCore.Mvc;

namespace DatabaseMastery.TransportMongoDb.Controllers
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class TrackingController : Controller
    {
        private readonly IShipmentService _shipmentService;

        public TrackingController(IShipmentService shipmentService)
        {
            _shipmentService = shipmentService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            ApplyTrackingPrivacyHeaders();

            if (Request.Query.ContainsKey("trackingNumber"))
            {
                return RedirectToAction(nameof(Index));
            }

            ViewBag.HasSearched = false;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(string? trackingNumber)
        {
            ApplyTrackingPrivacyHeaders();

            ViewBag.HasSearched = true;
            ViewBag.SearchedNumber = trackingNumber ?? string.Empty;

            if (string.IsNullOrWhiteSpace(trackingNumber))
            {
                ViewBag.ValidationMessage = "Lütfen takip numaranızı girin.";
                return View();
            }

            if (trackingNumber.Length > 64)
            {
                ViewBag.ValidationMessage = "Takip numarası en fazla 64 karakter olabilir.";
                return View();
            }

            var trackingResult = await _shipmentService
                .GetPublicTrackingByTrackingNumberAsync(trackingNumber.Trim());

            return View(trackingResult);
        }

        private void ApplyTrackingPrivacyHeaders()
        {
            Response.Headers["Referrer-Policy"] = "no-referrer";
            Response.Headers["Expires"] = "0";
        }
    }
}
