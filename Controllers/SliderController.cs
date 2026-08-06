using Microsoft.AspNetCore.Mvc;
using DatabaseMastery.TransportMongoDb.Services.SliderServices;
using DatabaseMastery.TransportMongoDb.Dtos.SliderDto;

namespace DatabaseMastery.TransportMongoDb.Controllers
{
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
        public IActionResult CreateSlider()
        {
            return View("~/Views/AdminLayout/CreateSlider.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> CreateSlider(CreateSliderDto createSliderDto)
        {
            await _sliderService.CreateSliderAsync(createSliderDto);
            return RedirectToAction(nameof(Sliderlist));
        }

        public async Task<IActionResult> DeleteSlider(string id)
        {
            await _sliderService.DeleteSliderAsync(id);
            return RedirectToAction(nameof(Sliderlist));
        }

        [HttpGet]
        public async Task<IActionResult> UpdateSlider(string id)
        {
            var values = await _sliderService.GetSliderByIdAsync(id);
            return View("~/Views/AdminLayout/UpdateSlider.cshtml", values);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSlider(UpdateSliderDto updateSliderDto)
        {
            await _sliderService.UpdateSliderAsync(updateSliderDto);
            return RedirectToAction(nameof(Sliderlist));
        }
    }
}
