using DatabaseMastery.TransportMongoDb.Dtos.SliderDto;
using DatabaseMastery.TransportMongoDb.Security;
using DatabaseMastery.TransportMongoDb.Services.SliderServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

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
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AdminRoles.AdminsOnly)]
        public async Task<IActionResult> CreateSlider(CreateSliderDto createSliderDto)
        {
            if (!ModelState.IsValid)
            {
                return View(
                    "~/Views/AdminLayout/CreateSlider.cshtml",
                    createSliderDto);
            }

            await _sliderService.CreateSliderAsync(createSliderDto);
            return RedirectToAction(nameof(Sliderlist));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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
            if (!ObjectId.TryParse(id, out _))
            {
                return NotFound();
            }

            var values = await _sliderService.GetSliderByIdAsync(id);
            if (values is null || string.IsNullOrWhiteSpace(values.SliderId))
            {
                return NotFound();
            }

            var updateDto = new UpdateSliderDto
            {
                SliderId = values.SliderId,
                SliderTitle = values.SliderTitle,
                Subtitle = values.Subtitle,
                Description = values.Description,
                ImageUrl = values.ImageUrl
            };

            return View("~/Views/AdminLayout/UpdateSlider.cshtml", updateDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AdminRoles.AdminsOnly)]
        public async Task<IActionResult> UpdateSlider(
            string id,
            UpdateSliderDto updateSliderDto)
        {
            if (!ObjectId.TryParse(id, out _) ||
                !ObjectId.TryParse(updateSliderDto.SliderId, out _) ||
                !string.Equals(
                    id,
                    updateSliderDto.SliderId,
                    StringComparison.Ordinal))
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(
                    "~/Views/AdminLayout/UpdateSlider.cshtml",
                    updateSliderDto);
            }

            await _sliderService.UpdateSliderAsync(updateSliderDto);
            return RedirectToAction(nameof(Sliderlist));
        }
    }
}
