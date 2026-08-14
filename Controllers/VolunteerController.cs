using Microsoft.AspNetCore.Mvc;

namespace GiftOfTheGivers.Controllers
{
    public class VolunteerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
