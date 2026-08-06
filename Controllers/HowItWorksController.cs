using DatabaseMastery.TransportMongoDb.Dtos.HowItWorksDto;
using DatabaseMastery.TransportMongoDb.Security;
using DatabaseMastery.TransportMongoDb.Services.HowItWorksServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        [Authorize(Roles = AdminRoles.AdminsOnly)]
        public async Task<IActionResult> CreateHowItWorks(
            CreateHowItWorksDto createHowItWorksDto)
        {
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
            var values = await _howItWorksService.GetHowItWorksByIdAsync(id);
            return View("~/Views/AdminLayout/UpdateHowItWorks.cshtml", values);
        }

        [HttpPost]
        [Authorize(Roles = AdminRoles.AdminsOnly)]
        public async Task<IActionResult> UpdateHowItWorks(
            UpdateHowItWorksDto updateHowItWorksDto)
        {
            await _howItWorksService.UpdateHowItWorksAsync(updateHowItWorksDto);
            return RedirectToAction(nameof(HowItWorkslist));
        }
    }
}
