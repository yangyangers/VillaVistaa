using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VillaVista.Data;
using VillaVista.Models.ViewModels;

namespace VillaVista.Controllers
{
    public class AdminDashboardController : Controller
    {
        private readonly HomeownerDbContext _dbContext;  // Add this field

        public AdminDashboardController(HomeownerDbContext dbContext)  // Add constructor
        {
            _dbContext = dbContext;
        }
        public IActionResult Index() => View();

        public IActionResult LoadView(string view)
        {
            if (view == "Homeowners")
            {
                var homeownerData = _dbContext.Homeowners
                    .Include(h => h.User)
                    .ToList(); // Force EF to execute the query and load data into memory

                var homeowners = homeownerData
                    .Select(homeowner => new HomeownerDisplayViewModel
                    {
                        UserId = homeowner.UserId,
                        FirstName = homeowner.User.Fullname?.Split(' ').FirstOrDefault() ?? "",
                        LastName = homeowner.User.Fullname?.Split(' ').Length > 1
                            ? string.Join(" ", homeowner.User.Fullname.Split(' ').Skip(1))
                            : "",
                        Email = homeowner.User.Email,
                        PhoneNumber = homeowner.User.PhoneNumber,
                        HouseNumber = homeowner.HouseNumber,
                        MoveInDate = homeowner.MoveInDate,
                        Status = homeowner.Status,
                        Notes = homeowner.Notes
                    })
                    .ToList();

                return PartialView("_Homeowners", homeowners);
            }

            return PartialView($"_{view}");
        }
        public IActionResult LoadNotifications()
        {
            return PartialView("_NotificationsPartial");
        }
    }
}
