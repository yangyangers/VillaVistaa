using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using VillaVista.Models;
using VillaVista.Models.ViewModels;

public class AdminController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpPost]
    public async Task<IActionResult> AddUser(RegisterUserViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
                Fullname = model.FullName
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, model.Role);
                return Json(new { success = true, message = $"{model.Role} added successfully!" });
            }
            return Json(new { success = false, message = "Error creating user." });
        }
        return Json(new { success = false, message = "Invalid data." });
    }

    public async Task<IActionResult> ResetAdminPassword()
    {
        var adminUser = await _userManager.FindByEmailAsync("admin@villavista.com");
        if (adminUser == null)
        {
            return Content("Admin user not found.");
        }

        string resetToken = await _userManager.GeneratePasswordResetTokenAsync(adminUser);
        IdentityResult resetResult = await _userManager.ResetPasswordAsync(adminUser, resetToken, "Admin@123");

        if (resetResult.Succeeded)
        {
            return Content("Admin password reset successfully. Use 'Admin@123' to log in.");
        }
        else
        {
            return Content("Failed to reset admin password.");
        }
    }


}
