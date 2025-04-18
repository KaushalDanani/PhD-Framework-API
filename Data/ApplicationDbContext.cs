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

            // Faculty Entity
            builder.Entity<Faculty>()
                .HasIndex(f => f.Email)
                .IsUnique();

            builder.Entity<Faculty>().HasData(
                new Faculty
                {
                    Id = 1,
                    Name = "Faculty of Science",
                    Description =
                        "The Faculty of Science is a Constituent Institution of the Maharaja Sayajirao University of Baroda under the direct management and control of the University. The Old Baroda College which was founded in the year 1881 consisted of Arts and Science Sections.",
                    ContactNo = "0265-2795329",
                    Email = "dean-science@msubaroda.ac.in",
                    Location = "Faculty of Science University Campus,Sayajigunj,Baroda-390002",
                    CreatedAt = DateTime.Now
                },
                new Faculty
                {
                    Id = 2,
                    Name = "Faculty of Arts",
                    Description =
                        "The annals of the Faculty of Arts lie nestled in the grand narrative of the erstwhile Baroda College [Established-1881] by H.H. Maharaja Sayajirao Gaekwad III. In 1949, a conglomerate of several institutions amalgamated to constitute The Maharaja Sayajirao University of Baroda, and Baroda College emerged as Faculty of Arts.",
                    ContactNo = "0987654321",
                    Email = "dean-arts@msubaroda.ac.in",
                    Location = "Near Kalaghoda, University Campus, Sayajigunj, Baroda-390002",
                    CreatedAt = DateTime.Now
                },
                new Faculty
                {
                    Id = 3,
                    Name = "Faculty of Technology and Engineering",
                    Description =
                        "The Faculty of Technology and Engineering as its stands today was formed along with the establishment of The Maharaja Sayajirao University in 1949. It  is an outgrowth of what was popularly known as the Kala Bhavan Technical Institute (KBTI) established in June 1890 by late His Highness The Maharaja Sayajirao Gaekwad III of Baroda State",
                    ContactNo = "0265-2434188",
                    Email = "dean-techo@msubaroda.ac.in",
                    Location = "Kalabhavan,Rajmahal Road,Baroda-390001",
                    CreatedAt = DateTime.Now
                },
                new Faculty
                {
                    Id = 4,
                    Name = "Faculty of Law",
                    Description =
                        "The MS University of Baroda set up in 1949 was the dream project of the visionary ruler Sir Sayaji Rao Gaekwad  III. It is an internationally renowned University with about 35000 students drawn from all over the world.",
                    ContactNo = "0265-2795503",
                    Email = "lawfacultymsu@gmail.com",
                    Location = "Maharaja Pratapsinhrao Gaekwad Parisar,,Opp.University Head Office,Baroda-390002",
                    CreatedAt = DateTime.Now
                }
            );

            // Dean Entity
            builder.Entity<Dean>()
                .HasIndex(d => d.Email)
                .IsUnique();

            builder.Entity<Dean>()
                .HasOne(d => d.Faculty)
                .WithMany()
                .HasForeignKey(d => d.FacultyId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            builder.Entity<Dean>()
                .HasOne(d => d.ApplicationUser)
                .WithOne()
                .HasForeignKey<Dean>(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            // FacultyId added to guide and student
            builder.Entity<Student>()
                .HasOne(s => s.Faculty)
                .WithMany()
                .HasForeignKey(s => s.FacultyId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            builder.Entity<Guide>()
                .HasOne(g => g.Faculty)
                .WithMany()
                .HasForeignKey(g => g.FacultyId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            // Department Entity
            builder.Entity<Department>()
                .HasOne(d => d.Faculty)
                .WithMany()
                .HasForeignKey(d => d.FacultyId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            builder.Entity<Department>().HasData(
                new Department
                {
                    Id = 1,
                    FacultyId = 1,
                    Name = "Department of Physics",
                    Email = "physics@sciencefaculty.edu",
                    Location = "Science Block A, Room 101",
                    ContactNo = "123-456-7894"
                },
                new Department
                {
                    Id = 2,
                    FacultyId = 1,
                    Name = "Department of Chemistry",
                    Email = "chemistry@sciencefaculty.edu",
                    Location = "Science Block B, Room 102",
                    ContactNo = "123-456-7895"
                },
                new Department
                {
                    Id = 3,
                    FacultyId = 1,
                    Name = "Department of Biology",
                    Email = "biology@sciencefaculty.edu",
                    Location = "Science Block C, Room 103",
                    ContactNo = "123-456-7896"
                },
                new Department
                {
                    Id = 4,
                    FacultyId = 2,
                    Name = "Department of History",
                    Email = "history@artsfaculty.edu",
                    Location = "Arts Building A, Room 201",
                    ContactNo = "123-456-7897"
                },
                new Department
                {
                    Id = 5,
                    FacultyId = 2,
                    Name = "Department of Philosophy",
                    Email = "philosophy@artsfaculty.edu",
                    Location = "Arts Building B, Room 202",
                    ContactNo = "123-456-7898"
                },
                new Department
                {
                    Id = 6,
                    FacultyId = 2,
                    Name = "Department of Literature",
                    Email = "literature@artsfaculty.edu",
                    Location = "Arts Building C, Room 203",
                    ContactNo = "123-456-7899"
                },
                new Department
                {
                    Id = 7,
                    FacultyId = 3,
                    Name = "Department of Computer Science",
                    Email = "cs@techuniversity.edu",
                    Location = "Block A, Floor 2",
                    ContactNo = "123-456-7890"
                },
                new Department
                {
                    Id = 8,
                    FacultyId = 3,
                    Name = "Department of Mechanical Engineering",
                    Email = "mech@techuniversity.edu",
                    Location = "Block B, Floor 1",
                    ContactNo = "123-456-7891"
                },
                new Department
                {
                    Id = 9,
                    FacultyId = 3,
                    Name = "Department of Civil Engineering",
                    Email = "civil@techuniversity.edu",
                    Location = "Block C, Floor 3",
                    ContactNo = "123-456-7892"
                },
                new Department
                {
                    Id = 10,
                    FacultyId = 4,
                    Name = "Department of Law",
                    Email = "law@lawfaculty.edu",
                    Location = "Law Building, Room 101",
                    ContactNo = "123-456-7893"
                }
            );

            // DepartmentId added to guide and student
            builder.Entity<Student>()
                .HasOne(s => s.Department)
                .WithMany()
                .HasForeignKey(s => s.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            builder.Entity<Guide>()
                .HasOne(g => g.Department)
                .WithMany()
                .HasForeignKey(g => g.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
        }

        public static void SeedRoles(RoleManager<ApplicationRole> roleManager)
        {
            var roleNames = new[] { "student", "guide", "dean of faculty", "academic section" };

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
        public DbSet<Faculty> Faculties { get; set; }
        public DbSet<Dean> Deans { get; set; }
        public DbSet<Department> Departments { get; set; }

    }
}
