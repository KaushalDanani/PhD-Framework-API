using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Backend.Entities
{
    public class ApplicationUser: IdentityUser
    {
        [Required]
        public string ApplicationRoleId { get; set; }
        public ApplicationRole Role { get; set; }

        [Required]
        public Guid? ProfileImageId { get; set; }
        public virtual ApplicationFile ProfileImage { get; set; }

        public bool IsProfileImageChanged { get; set; } = false;
    }
}
