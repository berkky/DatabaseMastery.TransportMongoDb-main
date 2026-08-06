 using DatabaseMastery.TransportMongoDb.Dtos.AboutDto;
using DatabaseMastery.TransportMongoDb.Services.AboutServices;
using Microsoft.AspNetCore.Mvc;

namespace DatabaseMastery.TransportMongoDb.Controllers
{
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
        public IActionResult CreateAbout()
        {
            return View("~/Views/AdminLayout/CreateAbout.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> CreateAbout(CreateAboutDto createAboutDto)
        {
            await _AboutService.CreateAboutAsync(createAboutDto);
            return RedirectToAction(nameof(Aboutlist));
        }

        public async Task<IActionResult> DeleteAbout(string id)
        {
            await _AboutService.DeleteAboutAsync(id);
            return RedirectToAction(nameof(Aboutlist));
        }

        [HttpGet]
        public async Task<IActionResult> UpdateAbout(string id)
        {
            var values = await _AboutService.GetAboutByIdAsync(id);
            return View("~/Views/AdminLayout/UpdateAbout.cshtml", values);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAbout(UpdateAboutDto updateAboutDto)
        {
            await _AboutService.UpdateAboutAsync(updateAboutDto);
            return RedirectToAction(nameof(Aboutlist));
        }
    }
}
