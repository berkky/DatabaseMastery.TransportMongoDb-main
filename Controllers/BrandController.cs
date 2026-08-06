using DatabaseMastery.TransportMongoDb.Dtos.BrandDtos;
using DatabaseMastery.TransportMongoDb.Security;
using DatabaseMastery.TransportMongoDb.Services.BrandServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        [Authorize(Roles = AdminRoles.AdminsOnly)]
        public async Task<IActionResult> CreateBrand(CreateBrandDto createBrandDto)
        {
            await _BrandService.CreateBrandAsync(createBrandDto);
            return RedirectToAction(nameof(Brandlist));
        }

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
            var values = await _BrandService.GetBrandByIdAsync(id);
            return View("~/Views/AdminLayout/UpdateBrand.cshtml", values);
        }

        [HttpPost]
        [Authorize(Roles = AdminRoles.AdminsOnly)]
        public async Task<IActionResult> UpdateBrand(UpdateBrandDto updateBrandDto)
        {
            await _BrandService.UpdateBrandAsync(updateBrandDto);
            return RedirectToAction(nameof(Brandlist));
        }
    }
}
