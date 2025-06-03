using Microsoft.AspNetCore.Identity;

namespace EnglishApp.Data
{
    public class Role : IdentityRole<int>
    {
        public ICollection<UserRole> UserRoles { get; set; } = null!;

    }
}
