using DatabaseMastery.TransportMongoDb.Dtos.ShipmentTrackingDtos;
using DatabaseMastery.TransportMongoDb.Security;
using DatabaseMastery.TransportMongoDb.Services.ShipmentServices;
using DatabaseMastery.TransportMongoDb.Services.ShipmentTrackingServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatabaseMastery.TransportMongoDb.Controllers
{
    [Authorize(Roles = AdminRoles.AllAdminRoles)]
    public class ShipmentTrackingController : Controller
    {
        private readonly IShipmentTrackingService _trackingService;
        private readonly IShipmentService _shipmentService;

        public ShipmentTrackingController(
            IShipmentTrackingService trackingService,
            IShipmentService shipmentService)
        {
            _trackingService = trackingService;
            _shipmentService = shipmentService;
        }

        public async Task<IActionResult> Index(string trackingNumber)
        {
            var values = await _trackingService
                .GetAllTrackingsAsync(trackingNumber);

            ViewBag.TrackingNumber = trackingNumber;
            return View(values);
        }

        [HttpGet]
        [Authorize(Roles = AdminRoles.OperatorsAndAbove)]
        public async Task<IActionResult> AddTracking(string trackingNumber)
        {
            if (string.IsNullOrWhiteSpace(trackingNumber))
            {
                return BadRequest();
            }

            if (!await PopulateShipmentSummaryAsync(trackingNumber))
            {
                return NotFound();
            }

            return View(new CreateShipmentTrackingDto
            {
                TrackingNumber = trackingNumber,
                EventDate = DateTime.Now
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AdminRoles.OperatorsAndAbove)]
        public async Task<IActionResult> AddTracking(
            CreateShipmentTrackingDto createDto)
        {
            if (createDto.EventDate == default)
            {
                ModelState.AddModelError(
                    nameof(createDto.EventDate),
                    "Hareket tarihi gereklidir.");
            }

            if (!ModelState.IsValid)
            {
                if (!await PopulateShipmentSummaryAsync(createDto.TrackingNumber))
                {
                    return NotFound();
                }

                return View(createDto);
            }

            await _trackingService.CreateTrackingAsync(createDto);

            TempData["Success"] = "Kargo hareketi başarıyla eklendi.";

            return RedirectToAction("Index", new
            {
                trackingNumber = createDto.TrackingNumber
            });
        }

        [HttpGet]
        [Authorize(Roles = AdminRoles.OperatorsAndAbove)]
        public async Task<IActionResult> UpdateTracking(
            string trackingNumber, int index)
        {
            if (string.IsNullOrWhiteSpace(trackingNumber) || index < 0)
            {
                return BadRequest();
            }

            var tracking = await _trackingService
                .GetTrackingByIndexAsync(trackingNumber, index);

            if (tracking == null)
            {
                return NotFound();
            }

            if (!await PopulateShipmentSummaryAsync(trackingNumber))
            {
                return NotFound();
            }

            var updateDto = new UpdateShipmentTrackingDto
            {
                TrackingNumber = trackingNumber,
                TrackingIndex = index,
                EventDate = tracking.EventDate,
                Location = tracking.Location,
                Description = tracking.Description,
                TrackingStatus = tracking.TrackingStatus
            };

            return View(updateDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AdminRoles.OperatorsAndAbove)]
        public async Task<IActionResult> UpdateTracking(
            UpdateShipmentTrackingDto updateDto)
        {
            if (updateDto.EventDate == default)
            {
                ModelState.AddModelError(
                    nameof(updateDto.EventDate),
                    "Hareket tarihi gereklidir.");
            }

            if (!ModelState.IsValid)
            {
                if (!await PopulateShipmentSummaryAsync(updateDto.TrackingNumber))
                {
                    return NotFound();
                }

                return View(updateDto);
            }

            await _trackingService.UpdateTrackingAsync(updateDto);

            TempData["Success"] = "Kargo hareketi başarıyla güncellendi.";

            return RedirectToAction("Index", new
            {
                trackingNumber = updateDto.TrackingNumber
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AdminRoles.AdminsOnly)]
        public async Task<IActionResult> DeleteTracking(
            string trackingNumber, int index)
        {
            await _trackingService.DeleteTrackingAsync(trackingNumber, index);

            TempData["Success"] = "Kargo hareketi başarıyla silindi.";

            return RedirectToAction("Index", new { trackingNumber });
        }

        private async Task<bool> PopulateShipmentSummaryAsync(string trackingNumber)
        {
            var shipment = await _shipmentService
                .GetShipmentByTrackingNumberAsync(trackingNumber);

            if (shipment == null)
            {
                return false;
            }

            ViewBag.TrackingNumber = trackingNumber;
            ViewBag.SenderName = shipment.SenderName;
            ViewBag.ReceiverName = shipment.ReceiverName;
            ViewBag.OriginCity = shipment.DepartureCity;
            ViewBag.DestinationCity = shipment.ArrivalCity;
            ViewBag.CurrentStatus = shipment.CurrentStatus;
            ViewBag.ExistingTrackings = shipment.Trackings
                .OrderByDescending(x => x.EventDate)
                .ToList();

            return true;
        }
    }
}
