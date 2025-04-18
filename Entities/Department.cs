using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Entities
{
    public class Department
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [ForeignKey("Faculty")]
        public int FacultyId { get; set; }

        [Required]
        public string Name { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; }
        public string Location { get; set; }
        [Required, Phone]
        public string ContactNo { get; set; }

        public virtual Faculty Faculty { get; set; }
    }
}
