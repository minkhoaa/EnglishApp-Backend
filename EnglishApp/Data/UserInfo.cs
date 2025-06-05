using System.ComponentModel.DataAnnotations;

namespace EnglishApp.Data
{
    public class UserInfo
    {
        [Key]
        public int UserId{ get; set; }
        public string FullName { get; set; } = string.Empty;
        public DateTime Birthday { get; set; } 

        public string Email { get; set; } = string.Empty;
        public string NumberPhone { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;

        public string Gender { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;

        public User User { get; set; }
    }
}
