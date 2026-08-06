using Microsoft.AspNetCore.Mvc;
namespace DatabaseMastery.TransportMongoDb.ViewComponents.DefaultComponents
{
    public class _DefaultFooterComponentPartial: ViewComponent  
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
