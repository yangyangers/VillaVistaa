//AccountController.cs

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
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
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        HomeownerDbContext context,
        IEmailSender emailSender,
        ILogger<AccountController> logger)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
        _emailSender = emailSender;
        _logger = logger;
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
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddResident([FromForm] AddHomeownerViewModel model)
    {
        _logger.LogInformation("AddResident endpoint hit with data: {@Model}", model);

        if (!ModelState.IsValid)
        {
            return Json(new { success = false, message = "Please fill all required fields." });
        }

        // Check if email already exists
        var existingUser = await _userManager.FindByEmailAsync(model.Email);
        if (existingUser != null)
        {
            return Json(new { success = false, message = "Email is already in use." });
        }

        // Create a user account for the resident
        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            Fullname = $"{model.FirstName} {model.LastName}",
            PhoneNumber = model.Phone,
            EmailConfirmed = true // Set to true to avoid email confirmation requirement
        };

        // Generate a temporary password
        string tempPassword = GenerateRandomPassword();

        var result = await _userManager.CreateAsync(user, tempPassword);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return Json(new { success = false, message = errors });
        }

        // Assign the Homeowner role
        await _userManager.AddToRoleAsync(user, "Homeowner");

        // Create a Homeowner record
        var homeowner = new Homeowner
        {
            UserId = user.Id,
            HouseNumber = model.UnitNumber,
            MoveInDate = model.MoveInDate,
            Status = model.Status,
            Notes = model.Notes
        };

        _context.Homeowners.Add(homeowner);
        await _context.SaveChangesAsync();

        // Return success response with temporary password
        return Json(new
        {
            success = true,
            message = "Homeowner added successfully!",
            tempPassword = tempPassword
        });
    }

    [HttpPost]
    public async Task<IActionResult> EditResident(EditHomeownerViewModel model)
    {
        if (!ModelState.IsValid)
        {
            // Optionally show an error message or redirect back
            return RedirectToAction("Residents", "AdminDashboard");
        }

        // 1. Update the ASP.NET Identity User
        var user = await _userManager.FindByIdAsync(model.Id);
        if (user == null)
        {
            return NotFound();
        }

        user.Fullname = $"{model.FirstName} {model.LastName}";
        user.Email = model.Email;
        user.UserName = model.Email;
        user.PhoneNumber = model.Phone;

        await _userManager.UpdateAsync(user);

        // 2. Update the Homeowner table
        var homeowner = await _context.Homeowners.FirstOrDefaultAsync(h => h.UserId == user.Id);
        if (homeowner != null)
        {
            homeowner.HouseNumber = model.UnitNumber;
            homeowner.MoveInDate = model.MoveInDate;
            homeowner.Status = model.Status;
            homeowner.Notes = model.Notes;

            await _context.SaveChangesAsync();
        }

        // 3. Redirect back to the resident list
        return RedirectToAction("Residents", "AdminDashboard");
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteResident(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return BadRequest(new { success = false, message = "Invalid user ID." });
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound(new { success = false, message = "User not found." });
        }

        // Get the homeowner record
        var homeowner = await _context.Homeowners.FirstOrDefaultAsync(h => h.UserId == id);
        if (homeowner != null)
        {
            // Remove homeowner record
            _context.Homeowners.Remove(homeowner);
            await _context.SaveChangesAsync();
        }

        // Delete the user account
        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return BadRequest(new { success = false, message = errors });
        }

        return Json(new { success = true, message = "Homeowner deleted successfully." });
    }
    private string GenerateRandomPassword()
    {
        // Generate a random password that meets ASP.NET Identity requirements
        const string lowerChars = "abcdefghijklmnopqrstuvwxyz";
        const string upperChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string numbers = "0123456789";
        const string specialChars = "!@#$%^&*()";

        var random = new Random();
        var password = new string(
            // At least one uppercase
            Enumerable.Repeat(upperChars, 2).Select(s => s[random.Next(s.Length)]).Concat(
            // At least one lowercase
            Enumerable.Repeat(lowerChars, 2).Select(s => s[random.Next(s.Length)])).Concat(
            // At least one number
            Enumerable.Repeat(numbers, 2).Select(s => s[random.Next(s.Length)])).Concat(
            // At least one special character
            Enumerable.Repeat(specialChars, 2).Select(s => s[random.Next(s.Length)])).Concat(
            // Fill the rest with random characters
            Enumerable.Repeat(upperChars + lowerChars + numbers + specialChars, 4)
                .Select(s => s[random.Next(s.Length)])
            ).ToArray());

        // Shuffle the password characters
        return new string(password.OrderBy(x => random.Next()).ToArray());
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
            redirectUrl = Url.Action("Index", "AdminDashboard");
        }
        else if (roles.Contains("Staff"))
        {
            redirectUrl = Url.Action("Index", "StaffDashboard");
        }
        else if (roles.Contains("Homeowner"))
        {
            redirectUrl = Url.Action("Index", "HomeownerDashboard");
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