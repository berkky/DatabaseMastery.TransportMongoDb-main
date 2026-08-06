using DatabaseMastery.TransportMongoDb.Dtos.TestimonialDtos;
using DatabaseMastery.TransportMongoDb.Security;
using DatabaseMastery.TransportMongoDb.Services.TestimonialServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace DatabaseMastery.TransportMongoDb.Controllers
{
    [Authorize(Roles = AdminRoles.AllAdminRoles)]
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
        [Authorize(Roles = AdminRoles.AdminsOnly)]
        public IActionResult CreateTestimonial()
        {
            return View("~/Views/AdminLayout/CreateTestimonial.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AdminRoles.AdminsOnly)]
        public async Task<IActionResult> CreateTestimonial(
            CreateTestimonialDto createTestimonialDto)
        {
            if (!ModelState.IsValid)
            {
                return View(
                    "~/Views/AdminLayout/CreateTestimonial.cshtml",
                    createTestimonialDto);
            }

            await _testimonialService.CreateTestimonialAsync(createTestimonialDto);
            return RedirectToAction(nameof(TestimonialList));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AdminRoles.AdminsOnly)]
        public async Task<IActionResult> DeleteTestimonial(string id)
        {
            await _testimonialService.DeleteTestimonialAsync(id);
            return RedirectToAction(nameof(TestimonialList));
        }

        [HttpGet]
        [Authorize(Roles = AdminRoles.AdminsOnly)]
        public async Task<IActionResult> UpdateTestimonial(string id)
        {
            if (!ObjectId.TryParse(id, out _))
            {
                return NotFound();
            }

            var values = await _testimonialService.GetTestimonialByIdAsync(id);
            if (values is null || string.IsNullOrWhiteSpace(values.TestimonialId))
            {
                return NotFound();
            }

            var updateDto = new UpdateTestimonialDto
            {
                TestimonialId = values.TestimonialId,
                NameSurname = values.NameSurname,
                Title = values.Title,
                ImageUrl = values.ImageUrl,
                ReviewDetail = values.ReviewDetail,
                ReviewScore = values.ReviewScore,
                Status = values.Status
            };

            return View("~/Views/AdminLayout/UpdateTestimonial.cshtml", updateDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AdminRoles.AdminsOnly)]
        public async Task<IActionResult> UpdateTestimonial(
            string id,
            UpdateTestimonialDto updateTestimonialDto)
        {
            if (!ObjectId.TryParse(id, out _) ||
                !ObjectId.TryParse(updateTestimonialDto.TestimonialId, out _) ||
                !string.Equals(
                    id,
                    updateTestimonialDto.TestimonialId,
                    StringComparison.Ordinal))
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(
                    "~/Views/AdminLayout/UpdateTestimonial.cshtml",
                    updateTestimonialDto);
            }

            await _testimonialService.UpdateTestimonialAsync(updateTestimonialDto);
            return RedirectToAction(nameof(TestimonialList));
        }
    }
}
