using DatabaseMastery.TransportMongoDb.Dtos.ShipmentDtos;
using DatabaseMastery.TransportMongoDb.Security;
using DatabaseMastery.TransportMongoDb.Services.ShipmentServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MongoDB.Bson;

namespace DatabaseMastery.TransportMongoDb.Controllers
{
    [Authorize(Roles = AdminRoles.AllAdminRoles)]
    public class ShipmentController : Controller
    {
        private readonly IShipmentService _shipmentService;

        public ShipmentController(IShipmentService shipmentService)
        {
            _shipmentService = shipmentService;
        }

        public async Task<IActionResult> ShipmentList()
        {
            var values = await _shipmentService.GetAllShipmentsAsync();
            return View("~/Views/AdminLayout/ShipmentList.cshtml", values);
        }

        [HttpGet]
        [Authorize(Roles = AdminRoles.OperatorsAndAbove)]
        public IActionResult CreateShipment()
        {
            return View("~/Views/AdminLayout/CreateShipment.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AdminRoles.OperatorsAndAbove)]
        public async Task<IActionResult> CreateShipment(
            CreateShipmentDto createShipmentDto)
        {
            NormalizeTrackingNumber(
                createShipmentDto.TrackingNumber,
                nameof(createShipmentDto.TrackingNumber),
                value => createShipmentDto.TrackingNumber = value,
                ModelState);

            if (createShipmentDto.CreatedDate == default)
            {
                ModelState.AddModelError(
                    nameof(createShipmentDto.CreatedDate),
                    "Oluşturma tarihi gereklidir.");
            }

            if (!ModelState.IsValid)
            {
                return View(
                    "~/Views/AdminLayout/CreateShipment.cshtml",
                    createShipmentDto);
            }

            await _shipmentService.CreateShipmentAsync(createShipmentDto);
            return RedirectToAction(nameof(ShipmentList));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AdminRoles.AdminsOnly)]
        public async Task<IActionResult> DeleteShipment(string id)
        {
            await _shipmentService.DeleteShipmentAsync(id);
            return RedirectToAction(nameof(ShipmentList));
        }

        [HttpGet]
        [Authorize(Roles = AdminRoles.OperatorsAndAbove)]
        public async Task<IActionResult> UpdateShipment(string id)
        {
            if (!ObjectId.TryParse(id, out _))
            {
                return NotFound();
            }

            var values = await _shipmentService.GetShipmentByIdAsync(id);
            if (values is null || string.IsNullOrWhiteSpace(values.ShipmentId))
            {
                return NotFound();
            }

            var updateDto = new UpdateShipmentDto
            {
                ShipmentId = values.ShipmentId,
                TrackingNumber = values.TrackingNumber,
                SenderName = values.SenderName,
                SenderPhone = values.SenderPhone,
                ReceiverName = values.ReceiverName,
                ReceiverPhone = values.ReceiverPhone,
                DepartureCity = values.DepartureCity,
                DepartureDistrict = values.DepartureDistrict,
                ArrivalCity = values.ArrivalCity,
                ArrivalDistrict = values.ArrivalDistrict,
                Address = values.Address,
                CreatedDate = values.CreatedDate,
                CurrentStatus = values.CurrentStatus
            };

            return View("~/Views/AdminLayout/UpdateShipment.cshtml", updateDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AdminRoles.OperatorsAndAbove)]
        public async Task<IActionResult> UpdateShipment(
            string id,
            UpdateShipmentDto updateShipmentDto)
        {
            if (!ObjectId.TryParse(id, out _) ||
                !ObjectId.TryParse(updateShipmentDto.ShipmentId, out _) ||
                !string.Equals(
                    id,
                    updateShipmentDto.ShipmentId,
                    StringComparison.Ordinal))
            {
                return BadRequest();
            }

            NormalizeTrackingNumber(
                updateShipmentDto.TrackingNumber,
                nameof(updateShipmentDto.TrackingNumber),
                value => updateShipmentDto.TrackingNumber = value,
                ModelState);

            if (updateShipmentDto.CreatedDate == default)
            {
                ModelState.AddModelError(
                    nameof(updateShipmentDto.CreatedDate),
                    "Oluşturma tarihi gereklidir.");
            }

            if (!ModelState.IsValid)
            {
                return View(
                    "~/Views/AdminLayout/UpdateShipment.cshtml",
                    updateShipmentDto);
            }

            await _shipmentService.UpdateShipmentAsync(updateShipmentDto);
            return RedirectToAction(nameof(ShipmentList));
        }

        private static void NormalizeTrackingNumber(
            string? trackingNumber,
            string propertyName,
            Action<string> setCanonical,
            ModelStateDictionary modelState)
        {
            const string requiredMessage = "Takip numarası gereklidir.";
            const string maxLengthMessage =
                "Takip numarası en fazla 64 karakter olabilir.";

            if (trackingNumber is null)
            {
                modelState.AddModelError(propertyName, requiredMessage);
                return;
            }

            var trimmed = trackingNumber.Trim();
            setCanonical(trimmed);
            modelState.Remove(propertyName);

            if (string.IsNullOrEmpty(trimmed))
            {
                modelState.AddModelError(propertyName, requiredMessage);
                return;
            }

            if (trimmed.Length > 64)
            {
                modelState.AddModelError(propertyName, maxLengthMessage);
            }
        }
    }
}
