using System.Globalization;
using DatabaseMastery.TransportMongoDb.Services.PublicTrackingRateLimiting;
using DatabaseMastery.TransportMongoDb.Services.ShipmentServices;
using Microsoft.AspNetCore.Mvc;

namespace DatabaseMastery.TransportMongoDb.Controllers
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class TrackingController : Controller
    {
        private readonly IShipmentService _shipmentService;
        private readonly IPublicTrackingRateLimiter _publicTrackingRateLimiter;

        public TrackingController(
            IShipmentService shipmentService,
            IPublicTrackingRateLimiter publicTrackingRateLimiter)
        {
            _shipmentService = shipmentService;
            _publicTrackingRateLimiter = publicTrackingRateLimiter;
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
        public async Task<IActionResult> Index(
            string? trackingNumber,
            CancellationToken cancellationToken)
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

            var trimmedTrackingNumber = trackingNumber.Trim();

            var rateLimitResult = await _publicTrackingRateLimiter.AcquireAsync(
                HttpContext.Connection.RemoteIpAddress,
                cancellationToken);

            if (!rateLimitResult.IsAllowed)
            {
                var retrySeconds = Math.Max(
                    1,
                    (int)Math.Ceiling(rateLimitResult.RetryAfter.TotalSeconds));

                Response.StatusCode = StatusCodes.Status429TooManyRequests;
                Response.Headers.RetryAfter =
                    retrySeconds.ToString(CultureInfo.InvariantCulture);

                const string rateLimitMessage =
                    "Çok fazla takip sorgusu yapıldı. Lütfen kısa süre sonra tekrar deneyin.";

                ModelState.AddModelError(string.Empty, rateLimitMessage);
                ViewBag.ValidationMessage = rateLimitMessage;
                return View();
            }

            var trackingResult = await _shipmentService
                .GetPublicTrackingByTrackingNumberAsync(trimmedTrackingNumber);

            return View(trackingResult);
        }

        private void ApplyTrackingPrivacyHeaders()
        {
            Response.Headers["Referrer-Policy"] = "no-referrer";
            Response.Headers["Expires"] = "0";
        }
    }
}
