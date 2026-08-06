using Microsoft.AspNetCore.Mvc;
namespace DatabaseMastery.TransportMongoDb.ViewComponents.DefaultComponents
{
    public class _DefaultGetQuoteComponentPartial: ViewComponent    
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
