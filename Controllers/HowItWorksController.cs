using DatabaseMastery.TransportMongoDb.Dtos.HowItWorksDto;
using DatabaseMastery.TransportMongoDb.Security;
using DatabaseMastery.TransportMongoDb.Services.HowItWorksServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace DatabaseMastery.TransportMongoDb.Controllers
{
    [Authorize(Roles = AdminRoles.AllAdminRoles)]
    public class HowItWorksController : Controller
    {
        private readonly IHowItWorksService _howItWorksService;

        public HowItWorksController(IHowItWorksService howItWorksService)
        {
            _howItWorksService = howItWorksService;
        }

        public async Task<IActionResult> HowItWorkslist()
        {
            var values = await _howItWorksService.GetAllHowItWorksAsync();
            return View("~/Views/AdminLayout/HowItWorksList.cshtml", values);
        }

        [HttpGet]
        [Authorize(Roles = AdminRoles.AdminsOnly)]
        public IActionResult CreateHowItWorks()
        {
            return View("~/Views/AdminLayout/CreateHowItWorks.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AdminRoles.AdminsOnly)]
        public async Task<IActionResult> CreateHowItWorks(
            CreateHowItWorksDto createHowItWorksDto)
        {
            if (!ModelState.IsValid)
            {
                return View(
                    "~/Views/AdminLayout/CreateHowItWorks.cshtml",
                    createHowItWorksDto);
            }

            await _howItWorksService.CreateHowItWorksAsync(createHowItWorksDto);
            return RedirectToAction(nameof(HowItWorkslist));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AdminRoles.AdminsOnly)]
        public async Task<IActionResult> DeleteHowItWorks(string id)
        {
            await _howItWorksService.DeleteHowItWorksAsync(id);
            return RedirectToAction(nameof(HowItWorkslist));
        }

        [HttpGet]
        [Authorize(Roles = AdminRoles.AdminsOnly)]
        public async Task<IActionResult> UpdateHowItWorks(string id)
        {
            if (!ObjectId.TryParse(id, out _))
            {
                return NotFound();
            }

            var values = await _howItWorksService.GetHowItWorksByIdAsync(id);
            if (values is null || string.IsNullOrWhiteSpace(values.HowItWorksId))
            {
                return NotFound();
            }

            var updateDto = new UpdateHowItWorksDto
            {
                HowItWorksId = values.HowItWorksId,
                Title = values.Title,
                Description = values.Description,
                ImageUrl = values.ImageUrl
            };

            return View("~/Views/AdminLayout/UpdateHowItWorks.cshtml", updateDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AdminRoles.AdminsOnly)]
        public async Task<IActionResult> UpdateHowItWorks(
            string id,
            UpdateHowItWorksDto updateHowItWorksDto)
        {
            if (!ObjectId.TryParse(id, out _) ||
                !ObjectId.TryParse(updateHowItWorksDto.HowItWorksId, out _) ||
                !string.Equals(
                    id,
                    updateHowItWorksDto.HowItWorksId,
                    StringComparison.Ordinal))
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(
                    "~/Views/AdminLayout/UpdateHowItWorks.cshtml",
                    updateHowItWorksDto);
            }

            await _howItWorksService.UpdateHowItWorksAsync(updateHowItWorksDto);
            return RedirectToAction(nameof(HowItWorkslist));
        }
    }
}
