using Microsoft.AspNetCore.Identity;

namespace EnglishApp.Data
{
    public class User : IdentityUser<int>
    {
        public ICollection<UserRole> UserRoles { get; set; } = null!;


    }
}
