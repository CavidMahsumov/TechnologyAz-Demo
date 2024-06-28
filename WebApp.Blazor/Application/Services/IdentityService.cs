using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http;
using System.Net.Security;
using System.Threading.Tasks;
using WebApp.Blazor.Application.Services.Interfaces;
using WebApp.Domain.Models.User;
using WebApp.Extensions;

namespace WebApp.Blazor.Application.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly HttpClient _httpClient;
        private readonly ISyncLocalStorageService _syncLocalStorageService;
        private readonly AuthenticationStateProvider _authProvider;

        public IdentityService(HttpClient httpClient, ISyncLocalStorageService syncLocalStorageService, AuthenticationStateProvider authProvider)
        {
            _httpClient = httpClient;
            _syncLocalStorageService = syncLocalStorageService;
            _authProvider = authProvider;
        }

        public bool IsLoggedIn => !string.IsNullOrEmpty(GetUserToken());

        public string GetUserName()
        {
            return _syncLocalStorageService.GetUserName();
        }

        public string GetUserToken()
        {
            return _syncLocalStorageService.GetToken();
        }

        public async Task<bool> Login(string username, string password)
        {
            var req = new UserLoginRequest(username, password);
            var response = await _httpClient.PostGetResponseAysnc<UserLoginResponse, UserLoginRequest>("auth", req);
            if (!string.IsNullOrEmpty(response.UserToken)) //login success
            {
                _syncLocalStorageService.SetToken(response.UserToken);
                _syncLocalStorageService.SetUserName(response.UserName);
                ((Utils.AuthStateProvider)_authProvider).NotifyUserLogin(response.UserName);
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", response.UserToken);
                return true;
            }
            return false;
        }

        public void Logout()
        {
            _syncLocalStorageService.RemoveItem("token");
            _syncLocalStorageService.RemoveItem("username");
            ((Utils.AuthStateProvider)_authProvider).NotifyUserLogout();
            _httpClient.DefaultRequestHeaders.Authorization = null;


        }
    }
}
