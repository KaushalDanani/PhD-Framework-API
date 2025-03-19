using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Entities
{
    public class Student
    {
        [Key]
        public string RegistrationId { get; set; }

        [Required]
        [ForeignKey("ApplicationUser")]
        public string UserId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FatherName { get; set; }
        public string Email { get; set; }
        public string PhoneNo { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Department { get; set; }
        public string Faculty { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Pincode { get; set; }
        public string State { get; set; }
        public string Country { get; set; }

        public ApplicationUser ApplicationUser { get; set; }

    }
}
