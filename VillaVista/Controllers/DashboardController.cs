using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

public class DashboardController : Controller
{
    [Authorize(Roles = "Admin")]
    public IActionResult Admin()
    {
        return View();
    }

    [Authorize(Roles = "Staff")]
    public IActionResult Staff()
    {
        return View();
    }

    [Authorize(Roles = "Homeowner")]
    public IActionResult Homeowner()
    {
        return View();
    }
}
