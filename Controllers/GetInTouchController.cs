using DatabaseMastery.TransportMongoDb.Dtos.GetInTouchDto;
using DatabaseMastery.TransportMongoDb.Services.GetInTouchServices;
using Microsoft.AspNetCore.Mvc;

namespace DatabaseMastery.TransportMongoDb.Controllers
{
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
        public IActionResult CreateGetInTouch()
        {
            return View("~/Views/AdminLayout/CreateGetInTouch.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> CreateGetInTouch(CreateGetInTouchDto createGetInTouchDto)
        {
            await _GetInTouchService.CreateGetInTouchAsync(createGetInTouchDto);
            return RedirectToAction(nameof(GetInTouchlist));
        }

        public async Task<IActionResult> DeleteGetInTouch(string id)
        {
            await _GetInTouchService.DeleteGetInTouchAsync(id);
            return RedirectToAction(nameof(GetInTouchlist));
        }

        [HttpGet]
        public async Task<IActionResult> UpdateGetInTouch(string id)
        {
            var values = await _GetInTouchService.GetGetInTouchByIdAsync(id);
            return View("~/Views/AdminLayout/UpdateGetInTouch.cshtml", values);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateGetInTouch(UpdateGetInTouchDto updateGetInTouchDto)
        {
            await _GetInTouchService.UpdateGetInTouchAsync(updateGetInTouchDto);
            return RedirectToAction(nameof(GetInTouchlist));
        }
    }
}
