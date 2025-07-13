using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using DataService;
using Microsoft.IdentityModel.Tokens;
using ModelService;

namespace LogicService
{
    public class UsersLogic : IUsersLogic
    {
        private readonly IConfiguration _configuration;
        private readonly IUsersData _usersData;

        public UsersLogic(IConfiguration configuration, IUsersData usersData)
        {
            _configuration = configuration;
            _usersData = usersData;
        }

        public async Task<LoginResponseModel> Login(LoginModel user)
        {
            if (string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.PasswordHash))
                throw new ArgumentException("Email and password are required");

            List<LoginResponseModel> result = await _usersData.Login(user);
            
            if (result.Count == 1 && BCrypt.Net.BCrypt.Verify(user.PasswordHash, result[0].PasswordHash))
            {
                result[0].PasswordHash = "";
                result[0].Token = GenerateJWT(result[0]);
                return result[0];
            }

            throw new UnauthorizedAccessException("Invalid email or password");
        }

        public async Task<int> Register(RegisterModel user)
        {
            if (string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.PasswordHash))
                throw new ArgumentException("Email and password are required");

            var userId = await _usersData.Register(user);

            if (userId <= 0)
                throw new Exception("Failed to register user");

            return userId;
        }

        private string GenerateJWT(LoginResponseModel user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? ""));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
                new Claim(ClaimTypes.Role, user.Role ?? ""),
                new Claim(ClaimTypes.NameIdentifier, user.CustomerId.ToString())
            };

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(1440),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}