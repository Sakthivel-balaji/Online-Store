using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ModelService;
using OnlineStoreWebAPI.DataContext;

namespace DataService
{
    public class UsersData : IUsersData
    {
        private readonly OnlineStoreDbContext _dbContext;

        public UsersData(OnlineStoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<LoginResponseModel>> Login(LoginModel user)
        {
            try
            {
                var result = await _dbContext.Customers
                    .Where(u => u.Email == user.Email && u.IsDeleted == false)
                    .Select(u => new LoginResponseModel
                    {
                        CustomerId = u.CustomerId,
                        Role = u.Role,
                        PasswordHash = u.PasswordHash
                    })
                    .ToListAsync();

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching login data: {ex.Message}");
            }
        }

        public async Task<int> Register(RegisterModel user)
        {
            try
            {
                var newUser = new Customer
                {
                    UserName = user.UserName ?? "",
                    Email = user.Email ?? "",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash),
                    Role = "Customer",
                    CreatedAt = DateTime.UtcNow
                };

                await _dbContext.Customers.AddAsync(newUser);
                await _dbContext.SaveChangesAsync();

                return newUser.CustomerId;
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && (sqlEx.Number == 2627 || sqlEx.Number == 2601))
            {
                var message = sqlEx.Message;

                if (message.Contains("UQ_Customers_Email"))
                {
                    throw new ArgumentException("Email already exists. Please choose another email.");
                }
                else if (message.Contains("UQ_Customers_UserName"))
                {
                    throw new ArgumentException("Username already exists. Please choose another username.");
                }
                else
                {
                    throw new ArgumentException("A unique constraint was violated. Please check your input.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error registering user: {ex.Message}");
            }
        }
    }
}