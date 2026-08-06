using DatabaseMastery.TransportMongoDb.Dtos.TestimonialDtos;
using DatabaseMastery.TransportMongoDb.Services.TestimonialServices;
using Microsoft.AspNetCore.Mvc;

namespace DatabaseMastery.TransportMongoDb.Controllers
{
    public class TestimonialController : Controller
    {
        private readonly ITestimonialService _testimonialService;

        public TestimonialController(ITestimonialService testimonialService)
        {
            _testimonialService = testimonialService;
        }

        public async Task<IActionResult> TestimonialList()
        {
            var values = await _testimonialService.GetAllTestimonialAsync();
            return View("~/Views/AdminLayout/TestimonialList.cshtml", values);
        }

        [HttpGet]
        public IActionResult CreateTestimonial()
        {
            return View("~/Views/AdminLayout/CreateTestimonial.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> CreateTestimonial(
            CreateTestimonialDto createTestimonialDto)
        {
            await _testimonialService.CreateTestimonialAsync(createTestimonialDto);
            return RedirectToAction(nameof(TestimonialList));
        }

        public async Task<IActionResult> DeleteTestimonial(string id)
        {
            await _testimonialService.DeleteTestimonialAsync(id);
            return RedirectToAction(nameof(TestimonialList));
        }

        [HttpGet]
        public async Task<IActionResult> UpdateTestimonial(string id)
        {
            var values = await _testimonialService.GetTestimonialByIdAsync(id);
            return View("~/Views/AdminLayout/UpdateTestimonial.cshtml", values);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTestimonial(
            UpdateTestimonialDto updateTestimonialDto)
        {
            await _testimonialService.UpdateTestimonialAsync(updateTestimonialDto);
            return RedirectToAction(nameof(TestimonialList));
        }
    }
}
