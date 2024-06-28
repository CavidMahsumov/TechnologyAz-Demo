using System.Threading.Tasks;

namespace WebApp.Blazor.Application.Services.Interfaces
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
