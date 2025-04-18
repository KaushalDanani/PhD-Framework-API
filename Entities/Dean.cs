using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Entities
{
    public class Dean
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Faculty")]
        public int FacultyId { get; set; }

        [Required]
        [ForeignKey("ApplicationUser")]
        public string UserId { get; set; }

        [Required, MaxLength(50)]
        public string FirstName { get; set; }
        [Required, MaxLength(50)]
        public string LastName { get; set; }
        [MaxLength(50)]
        public string FatherName { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; }
        [Phone, MaxLength(20)]
        public string PhoneNo { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }
        [Required]
        public string Gender { get; set; }
        [MaxLength(255)]
        public string? Address { get; set; }
        [MaxLength(100)]
        public string City { get; set; }
        [MaxLength(20)]
        public string Pincode { get; set; }
        [MaxLength(100)]
        public string State { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public virtual Faculty Faculty { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}
