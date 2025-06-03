using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnglishApp.Data
{
    [Table("TempOtp")]
    public class TempOtp
    {
            [Key]
            public string Email { get; set; } = null!;
            public string Otp { get; set; } = null!;
            public string? Password { get; set; }
            public DateTime Expiration { get; set; }
        }
}
