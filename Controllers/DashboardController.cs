using Microsoft.AspNetCore.Mvc;

namespace GiftOfTheGivers.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
