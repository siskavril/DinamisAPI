using DinamisAPI.ViewModels;

namespace DinamisAPI.Services
{
    public interface IAuthServices
    {
        Task<ResponseAPI> Login(UserViewModel request);
    }
}
