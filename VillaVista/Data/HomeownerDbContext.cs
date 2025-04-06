using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VillaVista.Models;

namespace VillaVista.Data
{
    public class HomeownerDbContext : IdentityDbContext<ApplicationUser>
    {
        public HomeownerDbContext(DbContextOptions<HomeownerDbContext> options)
            : base(options)
        {
        }

        // ✅ Add these DbSet properties for Staff & Homeowner
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<Homeowner> Homeowners { get; set; }
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<Billing> Billings { get; set; }
        public DbSet<Event> Events { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ✅ Seed default roles (Admin, Staff, Homeowner)
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = "1A2B3C4D-1234-5678-ABCD-1234567890AB", Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole { Id = "2B3C4D5E-2345-6789-BCDE-2345678901BC", Name = "Staff", NormalizedName = "STAFF" },
                new IdentityRole { Id = "3C4D5E6F-3456-789A-CDEF-3456789012CD", Name = "Homeowner", NormalizedName = "HOMEOWNER" }
            );

            // ✅ Set relationships
            builder.Entity<Staff>()
                .HasOne(s => s.User)
                .WithMany()
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Homeowner>()
                .HasOne(h => h.User)
                .WithMany()
                .HasForeignKey(h => h.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
