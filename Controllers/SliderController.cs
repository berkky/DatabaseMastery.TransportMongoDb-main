using DatabaseMastery.TransportMongoDb.Dtos.SliderDto;
using DatabaseMastery.TransportMongoDb.Security;
using DatabaseMastery.TransportMongoDb.Services.SliderServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatabaseMastery.TransportMongoDb.Controllers
{
    [Authorize(Roles = AdminRoles.AllAdminRoles)]
    public class SliderController : Controller
    {
        private readonly ISliderService _sliderService;

        public SliderController(ISliderService sliderService)
        {
            _sliderService = sliderService;
        }

        public async Task<IActionResult> Sliderlist()
        {
            var values = await _sliderService.GetAllSlidersAsync();
            return View("~/Views/AdminLayout/SliderList.cshtml", values);
        }

        [HttpGet]
        [Authorize(Roles = AdminRoles.AdminsOnly)]
        public IActionResult CreateSlider()
        {
            return View("~/Views/AdminLayout/CreateSlider.cshtml");
        }

        [HttpPost]
        [Authorize(Roles = AdminRoles.AdminsOnly)]
        public async Task<IActionResult> CreateSlider(CreateSliderDto createSliderDto)
        {
            await _sliderService.CreateSliderAsync(createSliderDto);
            return RedirectToAction(nameof(Sliderlist));
        }

        [Authorize(Roles = AdminRoles.AdminsOnly)]
        public async Task<IActionResult> DeleteSlider(string id)
        {
            await _sliderService.DeleteSliderAsync(id);
            return RedirectToAction(nameof(Sliderlist));
        }

        [HttpGet]
        [Authorize(Roles = AdminRoles.AdminsOnly)]
        public async Task<IActionResult> UpdateSlider(string id)
        {
            var values = await _sliderService.GetSliderByIdAsync(id);
            return View("~/Views/AdminLayout/UpdateSlider.cshtml", values);
        }

        [HttpPost]
        [Authorize(Roles = AdminRoles.AdminsOnly)]
        public async Task<IActionResult> UpdateSlider(UpdateSliderDto updateSliderDto)
        {
            await _sliderService.UpdateSliderAsync(updateSliderDto);
            return RedirectToAction(nameof(Sliderlist));
        }
    }
}
