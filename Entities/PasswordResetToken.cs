using System.ComponentModel.DataAnnotations;

namespace Backend.Entities
{
    public class PasswordResetToken
    {
        [Key]
        public string Email { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        public DateTime ExpireTime { get; set; }
    }
}
