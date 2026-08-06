using DatabaseMastery.TransportMongoDb.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatabaseMastery.TransportMongoDb.Controllers
{
    [Authorize(Roles = AdminRoles.AllAdminRoles)]
    public class AdminLayoutController : Controller
    {
        public IActionResult Index()
        {
            return View("Dashboard");
        }
    }
}
