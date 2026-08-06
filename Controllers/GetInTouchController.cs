using DatabaseMastery.TransportMongoDb.Dtos.GetInTouchDto;
using DatabaseMastery.TransportMongoDb.Security;
using DatabaseMastery.TransportMongoDb.Services.GetInTouchServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        [Authorize(Roles = AdminRoles.AdminsOnly)]
        public async Task<IActionResult> CreateGetInTouch(CreateGetInTouchDto createGetInTouchDto)
        {
            await _GetInTouchService.CreateGetInTouchAsync(createGetInTouchDto);
            return RedirectToAction(nameof(GetInTouchlist));
        }

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
            var values = await _GetInTouchService.GetGetInTouchByIdAsync(id);
            return View("~/Views/AdminLayout/UpdateGetInTouch.cshtml", values);
        }

        [HttpPost]
        [Authorize(Roles = AdminRoles.AdminsOnly)]
        public async Task<IActionResult> UpdateGetInTouch(UpdateGetInTouchDto updateGetInTouchDto)
        {
            await _GetInTouchService.UpdateGetInTouchAsync(updateGetInTouchDto);
            return RedirectToAction(nameof(GetInTouchlist));
        }
    }
}
