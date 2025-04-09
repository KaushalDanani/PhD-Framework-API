using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Entities
{
    public class Guide
    {
        [Key]
        public int GuideId { get; set; }

        [Required]
        [ForeignKey("ApplicationUser")]
        public string UserId { get; set; }

        [Required, MaxLength(50)]
        public string LastName { get; set; }

        [Required, MaxLength(50)]
        public string FirstName { get; set; }

        [MaxLength(50)]
        public string FatherName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [MaxLength(100)]
        public string DepartmentName { get; set; }

        [MaxLength(100)]
        public string FacultyName { get; set; }

        [MaxLength(255)]
        public string Address { get; set; }

        [MaxLength(100)]
        public string City { get; set; }

        [MaxLength(100)]
        public string State { get; set; }

        [MaxLength(100)]
        public string Country { get; set; }

        [MaxLength(10)]
        public string PinCode { get; set; }

        [Required, Phone]
        public string ContactNo { get; set; }

        [MaxLength(100)]
        public string Designation { get; set; }

        public int StudentsLimit { get; set; }

        public int ExperienceYears { get; set; }

        [MaxLength(255)]
        public string Specialist { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.Now;  // Default to current timestamp

        public ApplicationUser ApplicationUser { get; set; }
    }

}
