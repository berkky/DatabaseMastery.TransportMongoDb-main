using DatabaseMastery.TransportMongoDb.Dtos.ShipmentDtos;
using DatabaseMastery.TransportMongoDb.Services.ShipmentServices;
using Microsoft.AspNetCore.Mvc;

namespace DatabaseMastery.TransportMongoDb.Controllers
{
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
        public IActionResult CreateShipment()
        {
            return View("~/Views/AdminLayout/CreateShipment.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> CreateShipment(
            CreateShipmentDto createShipmentDto)
        {
            await _shipmentService.CreateShipmentAsync(createShipmentDto);
            return RedirectToAction(nameof(ShipmentList));
        }

        [HttpGet]
        public async Task<IActionResult> DeleteShipment(string id)
        {
            await _shipmentService.DeleteShipmentAsync(id);
            return RedirectToAction(nameof(ShipmentList));
        }

        [HttpGet]
        public async Task<IActionResult> UpdateShipment(string id)
        {
            var values = await _shipmentService.GetShipmentByIdAsync(id);
            return View("~/Views/AdminLayout/UpdateShipment.cshtml", values);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateShipment(
            UpdateShipmentDto updateShipmentDto)
        {
            await _shipmentService.UpdateShipmentAsync(updateShipmentDto);
            return RedirectToAction(nameof(ShipmentList));
        }
    }
}
