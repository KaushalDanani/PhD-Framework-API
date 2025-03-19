using Backend.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data
{
    public class ApplicationDbContext: IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Student>()
                .HasOne(s => s.ApplicationUser)
                .WithMany()
                .HasForeignKey(u => u.UserId)
                .IsRequired();
        }

        public static void SeedRoles(RoleManager<ApplicationRole> roleManager)
        {
            var roleNames = new[] { "student", "guide", "dean of faculty" };

            foreach (var role in roleNames)
            {
                var roleExist = roleManager.RoleExistsAsync(role).Result;
                if (!roleExist)
                {
                    var createRole = new ApplicationRole(role);
                    var result = roleManager.CreateAsync(createRole).Result;
                    if (!result.Succeeded)
                    {
                        Console.WriteLine(
                            $"Failed to create role {role}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
            }
        }

        public DbSet<Student> Students { get; set; }
    }
}
