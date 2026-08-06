using DatabaseMastery.TransportMongoDb.Dtos.AboutDto;
using DatabaseMastery.TransportMongoDb.Security;
using DatabaseMastery.TransportMongoDb.Services.AboutServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace DatabaseMastery.TransportMongoDb.Controllers
{
    [Authorize(Roles = AdminRoles.AllAdminRoles)]
    public class AboutController : Controller
    {
        private readonly IAboutService _AboutService;

        public AboutController(IAboutService AboutService)
        {
            _AboutService = AboutService;
        }

        public async Task<IActionResult> Aboutlist()
        {
            var values = await _AboutService.GetAllAboutsAsync();
            return View("~/Views/AdminLayout/Aboutlist.cshtml", values);
        }

        [HttpGet]
        [Authorize(Roles = AdminRoles.AdminsOnly)]
        public IActionResult CreateAbout()
        {
            return View("~/Views/AdminLayout/CreateAbout.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AdminRoles.AdminsOnly)]
        public async Task<IActionResult> CreateAbout(CreateAboutDto createAboutDto)
        {
            if (!ModelState.IsValid)
            {
                return View(
                    "~/Views/AdminLayout/CreateAbout.cshtml",
                    createAboutDto);
            }

            await _AboutService.CreateAboutAsync(createAboutDto);
            return RedirectToAction(nameof(Aboutlist));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AdminRoles.AdminsOnly)]
        public async Task<IActionResult> DeleteAbout(string id)
        {
            await _AboutService.DeleteAboutAsync(id);
            return RedirectToAction(nameof(Aboutlist));
        }

        [HttpGet]
        [Authorize(Roles = AdminRoles.AdminsOnly)]
        public async Task<IActionResult> UpdateAbout(string id)
        {
            if (!ObjectId.TryParse(id, out _))
            {
                return NotFound();
            }

            var values = await _AboutService.GetAboutByIdAsync(id);
            if (values is null || string.IsNullOrWhiteSpace(values.AboutId))
            {
                return NotFound();
            }

            var updateDto = new UpdateAboutDto
            {
                AboutId = values.AboutId,
                Title = values.Title,
                Description = values.Description,
                ImageUrl = values.ImageUrl
            };

            return View("~/Views/AdminLayout/UpdateAbout.cshtml", updateDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AdminRoles.AdminsOnly)]
        public async Task<IActionResult> UpdateAbout(
            string id,
            UpdateAboutDto updateAboutDto)
        {
            if (!ObjectId.TryParse(id, out _) ||
                !ObjectId.TryParse(updateAboutDto.AboutId, out _) ||
                !string.Equals(
                    id,
                    updateAboutDto.AboutId,
                    StringComparison.Ordinal))
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(
                    "~/Views/AdminLayout/UpdateAbout.cshtml",
                    updateAboutDto);
            }

            await _AboutService.UpdateAboutAsync(updateAboutDto);
            return RedirectToAction(nameof(Aboutlist));
        }
    }
}
