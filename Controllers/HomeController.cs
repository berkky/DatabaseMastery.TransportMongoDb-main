using DatabaseMastery.TransportMongoDb.Infrastructure.Operations;
using DatabaseMastery.TransportMongoDb.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DatabaseMastery.TransportMongoDb.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var correlationId = HttpContext.Items[CorrelationIdMiddleware.ItemKey] as string
                ?? Activity.Current?.Id
                ?? HttpContext.TraceIdentifier;

            return View(new ErrorViewModel { RequestId = correlationId });
        }
    }
}
