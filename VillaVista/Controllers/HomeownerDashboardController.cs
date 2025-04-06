using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using VillaVista.Data;
using VillaVista.Models;
using VillaVista.ViewModels;

[Authorize(Roles = "Homeowner")]
public class HomeownerDashboardController : Controller
{
    private readonly HomeownerDbContext _context;

    public HomeownerDashboardController(HomeownerDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var viewModel = new HomeownerDashboardViewModel
        {
            Announcements = await _context.Announcements
                                         .OrderByDescending(a => a.DatePosted)
                                         .Take(3)
                                         .ToListAsync(),
            PaymentSummary = await _context.Billings
                                           .Where(b => b.HomeownerId == userId)
                                           .OrderByDescending(b => b.DueDate)
                                           .FirstOrDefaultAsync(),
            Events = await _context.Events
                                   .OrderBy(e => e.EventDate)
                                   .Take(5)
                                   .ToListAsync()
        };

        return PartialView("_HomeownerDashboard", viewModel);
    }
}
    

