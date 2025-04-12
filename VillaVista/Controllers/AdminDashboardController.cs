using Microsoft.AspNetCore.Mvc;

namespace VillaVista.Controllers
{
    public class AdminDashboardController : Controller
    {
        public IActionResult Index() => View();

        public IActionResult LoadView(string view)
        {
            return PartialView($"_{view}");
        }
        public IActionResult LoadNotifications()
        {
            return PartialView("_NotificationsPartial");
        }
    }
}
