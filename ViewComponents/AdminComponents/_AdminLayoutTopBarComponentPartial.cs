using Microsoft.AspNetCore.Mvc; 
namespace DatabaseMastery.TransportMongoDb.ViewComponents.AdminComponents
{
    public class _AdminLayoutTopBarComponentPartial : ViewComponent 
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }   
    }
}
