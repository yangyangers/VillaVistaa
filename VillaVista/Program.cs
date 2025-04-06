using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using VillaVista.Data;
using VillaVista.Models;
using VillaVista.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.SetBasePath(Directory.GetCurrentDirectory());
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// ✅ Register the database context
builder.Services.AddDbContext<HomeownerDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ✅ Add Identity with Roles
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddRoles<IdentityRole>()  // ✅ Ensure roles are added
    .AddEntityFrameworkStores<HomeownerDbContext>()
    .AddDefaultTokenProviders();  // ✅ This already configures authentication

// ❌ Removed extra authentication registration

builder.Services.AddControllersWithViews();

// Register IEmailSender
builder.Services.AddTransient<IEmailSender, EmailSender>(); // Add this line
var app = builder.Build();

// ✅ Ensure database is migrated before role creation
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<HomeownerDbContext>();
    dbContext.Database.Migrate();  // ✅ Apply pending migrations

    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

    await EnsureRolesAndAdminExist(roleManager, userManager);
}

// Middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();  // ✅ Ensures authentication is enabled
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

// ✅ Ensure Roles & Default Admin Exist
async Task EnsureRolesAndAdminExist(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
{
    string[] roles = { "Admin", "Staff", "Homeowner" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // ✅ Create a default Admin if no admin exists
    if (userManager.Users.All(u => u.Email != "admin@villavista.com"))
    {
        var adminUser = new ApplicationUser
        {
            UserName = "admin@villavista.com",
            Email = "admin@villavista.com",
            Fullname = "System Admin"
        };

        var result = await userManager.CreateAsync(adminUser, "Admin@123");

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}
