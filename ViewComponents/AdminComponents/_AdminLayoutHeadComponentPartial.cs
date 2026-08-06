using Microsoft.AspNetCore.Mvc; 
namespace DatabaseMastery.TransportMongoDb.ViewComponents.AdminComponents
{
    public class _AdminLayoutHeadComponentPartial : ViewComponent   
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
