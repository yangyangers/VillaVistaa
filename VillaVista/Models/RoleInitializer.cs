using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace VillaVista.Models
{
    public static class RoleInitializer
    {
        public static async Task EnsureRolesExist(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "Admin", "Staff", "Homeowner" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}
