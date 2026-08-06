using DatabaseMastery.TransportMongoDb.Dtos.BrandDtos;
using DatabaseMastery.TransportMongoDb.Security;
using DatabaseMastery.TransportMongoDb.Services.BrandServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace DatabaseMastery.TransportMongoDb.Controllers
{
    [Authorize(Roles = AdminRoles.AllAdminRoles)]
    public class BrandController : Controller
    {
        private readonly IBrandService _BrandService;

        public BrandController(IBrandService BrandService)
        {
            _BrandService = BrandService;
        }

        public async Task<IActionResult> Brandlist()
        {
            var values = await _BrandService.GetAllBrandsAsync();
            return View("~/Views/AdminLayout/BrandList.cshtml", values);
        }

        [HttpGet]
        [Authorize(Roles = AdminRoles.AdminsOnly)]
        public IActionResult CreateBrand()
        {
            return View("~/Views/AdminLayout/CreateBrand.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AdminRoles.AdminsOnly)]
        public async Task<IActionResult> CreateBrand(CreateBrandDto createBrandDto)
        {
            if (!ModelState.IsValid)
            {
                return View(
                    "~/Views/AdminLayout/CreateBrand.cshtml",
                    createBrandDto);
            }

            await _BrandService.CreateBrandAsync(createBrandDto);
            return RedirectToAction(nameof(Brandlist));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AdminRoles.AdminsOnly)]
        public async Task<IActionResult> DeleteBrand(string id)
        {
            await _BrandService.DeleteBrandAsync(id);
            return RedirectToAction(nameof(Brandlist));
        }

        [HttpGet]
        [Authorize(Roles = AdminRoles.AdminsOnly)]
        public async Task<IActionResult> UpdateBrand(string id)
        {
            if (!ObjectId.TryParse(id, out _))
            {
                return NotFound();
            }

            var values = await _BrandService.GetBrandByIdAsync(id);
            if (values is null || string.IsNullOrWhiteSpace(values.BrandId))
            {
                return NotFound();
            }

            var updateDto = new UpdateBrandDto
            {
                BrandId = values.BrandId,
                BrandName = values.BrandName,
                ImageUrl = values.ImageUrl,
                IsStatus = values.IsStatus
            };

            return View("~/Views/AdminLayout/UpdateBrand.cshtml", updateDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AdminRoles.AdminsOnly)]
        public async Task<IActionResult> UpdateBrand(
            string id,
            UpdateBrandDto updateBrandDto)
        {
            if (!ObjectId.TryParse(id, out _) ||
                !ObjectId.TryParse(updateBrandDto.BrandId, out _) ||
                !string.Equals(
                    id,
                    updateBrandDto.BrandId,
                    StringComparison.Ordinal))
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(
                    "~/Views/AdminLayout/UpdateBrand.cshtml",
                    updateBrandDto);
            }

            await _BrandService.UpdateBrandAsync(updateBrandDto);
            return RedirectToAction(nameof(Brandlist));
        }
    }
}
