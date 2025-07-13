using ModelService;

namespace LogicService
{
    public interface IUsersLogic
    {
        Task<int> Register(RegisterModel user);
        Task<LoginResponseModel> Login(LoginModel user);
    }
}
