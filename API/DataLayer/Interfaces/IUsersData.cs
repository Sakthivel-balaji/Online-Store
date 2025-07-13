using ModelService;

namespace DataService
{
    public interface IUsersData
    {
        Task<int> Register(RegisterModel user);
        Task<List<LoginResponseModel>> Login(LoginModel user);
    }
}
