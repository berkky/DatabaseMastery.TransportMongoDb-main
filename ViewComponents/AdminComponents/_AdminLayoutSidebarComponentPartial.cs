using Microsoft.AspNetCore.Mvc; 
namespace DatabaseMastery.TransportMongoDb.ViewComponents.AdminComponents
{
    public class _AdminLayoutSidebarComponentPartial : ViewComponent    
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }   
    }
}
