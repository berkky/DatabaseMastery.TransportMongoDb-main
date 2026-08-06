using DatabaseMastery.TransportMongoDb.Dtos.OfferDto;
using DatabaseMastery.TransportMongoDb.Security;
using DatabaseMastery.TransportMongoDb.Services.OfferServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace DatabaseMastery.TransportMongoDb.Controllers
{
    [Authorize(Roles = AdminRoles.AllAdminRoles)]
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
        [Authorize(Roles = AdminRoles.AdminsOnly)]
        public IActionResult CreateOffer()
        {
            return View("~/Views/AdminLayout/CreateOffer.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AdminRoles.AdminsOnly)]
        public async Task<IActionResult> CreateOffer(CreateOfferDto createOfferDto)
        {
            if (!ModelState.IsValid)
            {
                return View(
                    "~/Views/AdminLayout/CreateOffer.cshtml",
                    createOfferDto);
            }

            await _OfferService.CreateOfferAsync(createOfferDto);
            return RedirectToAction(nameof(Offerlist));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AdminRoles.AdminsOnly)]
        public async Task<IActionResult> DeleteOffer(string id)
        {
            await _OfferService.DeleteOfferAsync(id);
            return RedirectToAction(nameof(Offerlist));
        }

        [HttpGet]
        [Authorize(Roles = AdminRoles.AdminsOnly)]
        public async Task<IActionResult> UpdateOffer(string id)
        {
            if (!ObjectId.TryParse(id, out _))
            {
                return NotFound();
            }

            var values = await _OfferService.GetOfferByIdAsync(id);
            if (values is null || string.IsNullOrWhiteSpace(values.OfferId))
            {
                return NotFound();
            }

            var updateDto = new UpdateOfferDto
            {
                OfferId = values.OfferId,
                Title = values.Title,
                Description = values.Description,
                ImageUrl = values.ImageUrl,
                IsStatus = values.IsStatus
            };

            return View("~/Views/AdminLayout/UpdateOffer.cshtml", updateDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AdminRoles.AdminsOnly)]
        public async Task<IActionResult> UpdateOffer(
            string id,
            UpdateOfferDto updateOfferDto)
        {
            if (!ObjectId.TryParse(id, out _) ||
                !ObjectId.TryParse(updateOfferDto.OfferId, out _) ||
                !string.Equals(
                    id,
                    updateOfferDto.OfferId,
                    StringComparison.Ordinal))
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(
                    "~/Views/AdminLayout/UpdateOffer.cshtml",
                    updateOfferDto);
            }

            await _OfferService.UpdateOfferAsync(updateOfferDto);
            return RedirectToAction(nameof(Offerlist));
        }
    }
}
