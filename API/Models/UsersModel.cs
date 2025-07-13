namespace ModelService
{
    public class RegisterModel
    {
        public string? UserName { get ; set; }
        public string? Email { get ; set; }
        public string? PasswordHash { get ; set ; }
    }

    public class LoginModel
    {
        public string? Email { get; set; }
        public string? PasswordHash { get; set; }
    }

    public class LoginResponseModel
    {
        public string? Token { get; set; }
        public int CustomerId { get; set; }
        public string? Role { get; set; }
        public string? PasswordHash { get; set; }
    }
}