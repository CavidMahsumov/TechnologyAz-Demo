using System.Threading.Tasks;

namespace WebApp.Application.Services.Interfaces
{
    public interface IIdentityService
    {
        string GetUserName();
        string GetUserToken();
        bool IsLoggedIn { get; }
        Task<bool> Login(string username, string password);
        void Logout();  
    }
}
