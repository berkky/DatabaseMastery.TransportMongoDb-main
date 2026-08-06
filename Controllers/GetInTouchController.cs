using DatabaseMastery.TransportMongoDb.Dtos.GetInTouchDto;
using DatabaseMastery.TransportMongoDb.Security;
using DatabaseMastery.TransportMongoDb.Services.GetInTouchServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace DatabaseMastery.TransportMongoDb.Controllers
{
    [Authorize(Roles = AdminRoles.AllAdminRoles)]
    public class GetInTouchController : Controller
    {
        private readonly IGetInTouchServices _GetInTouchService;

        public GetInTouchController(IGetInTouchServices GetInTouchService)
        {
            _GetInTouchService = GetInTouchService;
        }

        public async Task<IActionResult> GetInTouchlist()
        {
            var values = await _GetInTouchService.GetAllGetInTouchesAsync();
            return View("~/Views/AdminLayout/GetInTouchList.cshtml", values);
        }

        [HttpGet]
        [Authorize(Roles = AdminRoles.AdminsOnly)]
        public IActionResult CreateGetInTouch()
        {
            return View("~/Views/AdminLayout/CreateGetInTouch.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AdminRoles.AdminsOnly)]
        public async Task<IActionResult> CreateGetInTouch(CreateGetInTouchDto createGetInTouchDto)
        {
            if (!ModelState.IsValid)
            {
                return View(
                    "~/Views/AdminLayout/CreateGetInTouch.cshtml",
                    createGetInTouchDto);
            }

            await _GetInTouchService.CreateGetInTouchAsync(createGetInTouchDto);
            return RedirectToAction(nameof(GetInTouchlist));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AdminRoles.AdminsOnly)]
        public async Task<IActionResult> DeleteGetInTouch(string id)
        {
            await _GetInTouchService.DeleteGetInTouchAsync(id);
            return RedirectToAction(nameof(GetInTouchlist));
        }

        [HttpGet]
        [Authorize(Roles = AdminRoles.AdminsOnly)]
        public async Task<IActionResult> UpdateGetInTouch(string id)
        {
            if (!ObjectId.TryParse(id, out _))
            {
                return NotFound();
            }

            var values = await _GetInTouchService.GetGetInTouchByIdAsync(id);
            if (values is null ||
                string.IsNullOrWhiteSpace(values.GetInTouchSectionId))
            {
                return NotFound();
            }

            var updateDto = new UpdateGetInTouchDto
            {
                GetInTouchSectionId = values.GetInTouchSectionId,
                BadgeTitle = values.BadgeTitle,
                MainTitle = values.MainTitle,
                Description = values.Description,
                Feature1Title = values.Feature1Title,
                Feature1Description = values.Feature1Description,
                Feature2Title = values.Feature2Title,
                Feature2Description = values.Feature2Description,
                ImageUrl = values.ImageUrl,
                Status = values.Status
            };

            return View("~/Views/AdminLayout/UpdateGetInTouch.cshtml", updateDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AdminRoles.AdminsOnly)]
        public async Task<IActionResult> UpdateGetInTouch(
            string id,
            UpdateGetInTouchDto updateGetInTouchDto)
        {
            if (!ObjectId.TryParse(id, out _) ||
                !ObjectId.TryParse(updateGetInTouchDto.GetInTouchSectionId, out _) ||
                !string.Equals(
                    id,
                    updateGetInTouchDto.GetInTouchSectionId,
                    StringComparison.Ordinal))
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(
                    "~/Views/AdminLayout/UpdateGetInTouch.cshtml",
                    updateGetInTouchDto);
            }

            await _GetInTouchService.UpdateGetInTouchAsync(updateGetInTouchDto);
            return RedirectToAction(nameof(GetInTouchlist));
        }
    }
}
