namespace EnglishApp.Model
{
    public class SignUpModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; } = null!;
    }
}
