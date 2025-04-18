using System.ComponentModel.DataAnnotations;

namespace Backend.Entities
{
    public class Faculty
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public string? Description { get; set; }

        [Required]
        [Phone]
        public string ContactNo { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MaxLength(255)]
        public string Location { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
