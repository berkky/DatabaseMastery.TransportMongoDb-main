using Microsoft.AspNetCore.Mvc; 
namespace DatabaseMastery.TransportMongoDb.ViewComponents.AdminComponents
{
    public class _AdminLayoutScriptsComponentPartial : ViewComponent   
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }   
    }
}
