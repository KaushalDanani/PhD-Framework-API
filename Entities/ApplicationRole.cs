using Microsoft.AspNetCore.Identity;

namespace Backend.Entities
{
    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole() : base() { }

        public ApplicationRole(string roleName) : base(roleName) { }
    }

}
