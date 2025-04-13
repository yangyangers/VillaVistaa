using Microsoft.AspNetCore.Mvc;

namespace VillaVista.Controllers;

    public class StaffDashboardController : Controller
    {
        public IActionResult Index() => View();

        public IActionResult LoadView(string view)
        {
            return PartialView($"_{view}");
        }
    }

