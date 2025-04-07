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

            builder.Entity<ApplicationUser>()
                .HasOne(u => u.ProfileImage)
                .WithMany()
                .HasForeignKey(u => u.ProfileImageId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Student>()
                .HasOne(s => s.ApplicationUser)
                .WithMany()
                .HasForeignKey(u => u.UserId)
                .IsRequired();

            builder.Entity<Guide>()
                .HasOne(g => g.ApplicationUser)
                .WithOne()
                .HasForeignKey<Guide>(g => g.UserId)
                .IsRequired();

            builder.Entity<PhDTitle>()
                .HasOne(pt => pt.Guide)
                .WithMany()
                .HasForeignKey(pt => pt.GuideId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            // Optional Relationship between PhDTitle and CoGuide
            builder.Entity<PhDTitle>()
                .HasOne(pt => pt.CoGuide)
                .WithMany()
                .HasForeignKey(pt => pt.CoGuideId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<PhDTitle>()
                .HasOne(pt => pt.Student)
                .WithOne()
                .HasForeignKey<PhDTitle>(pt => pt.RegistrationId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            // Define Composite Primary Key
            builder.Entity<ProgressReport>()
                .HasKey(pr => new { pr.RegistrationId, pr.ProgressReportNo });

            builder.Entity<ProgressReport>()
                .HasOne(pr => pr.Student)
                .WithMany()
                .HasForeignKey(pr => pr.RegistrationId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            builder.Entity<ProgressReport>()
                .HasOne(pr => pr.ApplicationFile)
                .WithMany()
                .HasForeignKey(pr => pr.FileId)
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
        public DbSet<Guide> Guides { get; set; }
        public DbSet<PhDTitle> PhDTitles { get; set; }
        public DbSet<ApplicationFile> ApplicationFiles { get; set; }
        public DbSet<ProgressReport> ProgressReports { get; set; }
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

    }
}
