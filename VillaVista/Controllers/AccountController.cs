using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using VillaVista.Data;
using VillaVista.Models;
using VillaVista.Models.ViewModels;

public class AccountController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly HomeownerDbContext _context;
    private readonly IEmailSender _emailSender;
    public AccountController(SignInManager<ApplicationUser> signInManager,
                             UserManager<ApplicationUser> userManager,
                             RoleManager<IdentityRole> roleManager,
                                HomeownerDbContext context, IEmailSender emailSender)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Register([FromForm] RegisterUserViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Invalid registration request.");
        }

        // Ensure role exists
        if (!await _roleManager.RoleExistsAsync(model.Role))
        {
            return BadRequest("Invalid role selected.");
        }

        var existingUser = await _userManager.FindByEmailAsync(model.Email);
        if (existingUser != null)
        {
            return BadRequest("Email is already in use.");
        }

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            Fullname = model.FullName // Ensure this matches the property in ApplicationUser model
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return BadRequest(errors);
        }

        // Assign role
        await _userManager.AddToRoleAsync(user, model.Role);

        // ✅ Insert into Staff or Homeowner Table
        if (model.Role == "Staff")
        {
            var staff = new Staff
            {
                UserId = user.Id,
                Position = model.Position // Make sure to add this in the ViewModel
            };
            _context.Staffs.Add(staff);
        }
        else if (model.Role == "Homeowner")
        {
            var homeowner = new Homeowner
            {
                UserId = user.Id,
                HouseNumber = model.HouseNumber // Ensure this exists in the ViewModel
            };
            _context.Homeowners.Add(homeowner);
        }

        await _context.SaveChangesAsync();

        return Json(new { success = true, message = "Registration successful!", redirectUrl = Url.Action("Index", "Home") });
    }


    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginViewModel model)
    {
        if (model == null || string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
        {
            return BadRequest("Invalid login request.");
        }

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            return Unauthorized("Invalid email or password.");
        }

        var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
        if (!result.Succeeded)
        {
            return Unauthorized("Invalid email or password.");
        }

        // ✅ Check user role and redirect accordingly
        var roles = await _userManager.GetRolesAsync(user);
        string redirectUrl = Url.Action("Index", "Home"); // Default redirect

        if (roles.Contains("Admin"))
        {
            redirectUrl = Url.Action("Admin", "Dashboard");
        }
        else if (roles.Contains("Staff"))
        {
            redirectUrl = Url.Action("Staff", "Dashboard");
        }
        else if (roles.Contains("Homeowner"))
        {
            redirectUrl = Url.Action("Homeowner", "Dashboard");
        }

        return Json(new { success = true, redirectUrl });
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordViewModel model)
    {
        if (string.IsNullOrEmpty(model.Email))
        {
            return BadRequest(new { success = false, message = "Email is required." });
        }

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            return BadRequest(new { success = false, message = "User not found." });
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var resetLink = Url.Action("ResetPassword", "Account", new { token, email = user.Email }, Request.Scheme);

        // Send email (You need to configure email sending)
        var emailBody = $"Click <a href='{resetLink}'>here</a> to reset your password.";
        await _emailSender.SendEmailAsync(user.Email, "Reset Password", emailBody);

        return Ok(new { success = true });
    }


    [HttpGet]
    public IActionResult ResetPassword(string token, string email)
    {
        if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
        {
            return BadRequest("Invalid password reset request.");
        }

        var model = new ResetPasswordViewModel { Token = token, Email = email };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword([FromForm] ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Invalid input.");
        }

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            return BadRequest("User not found.");
        }

        var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return BadRequest(errors);
        }

        return Json(new { success = true, message = "Password reset successful!" });
    }


}
