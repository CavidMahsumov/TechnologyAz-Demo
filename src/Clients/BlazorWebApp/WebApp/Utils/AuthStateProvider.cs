using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApp.Extensions;

namespace WebApp.Utils
{
    public class AuthStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorageService;
        private readonly HttpClient _httpClient;
        private readonly AuthenticationState anonymous;

        public AuthStateProvider(ILocalStorageService localStorageService, HttpClient httpClient, AuthenticationState authenticationState)
        {
            _localStorageService = localStorageService;
            _httpClient = httpClient;
            this.anonymous = authenticationState;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            string apiToken = await _localStorageService.GetToken();
            if (string.IsNullOrEmpty(apiToken))
            {
                return anonymous;
            }
            string userName=await _localStorageService.GetUserName();
            var cp = new ClaimsPrincipal(new ClaimsIdentity(new[] { 
            
                new Claim(ClaimTypes.Name, userName)
            },"jwtAuthType"));

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiToken);
            return new AuthenticationState(cp);
        }

        public void NotifyUserLogin(string userName)
        {
            var cp = new ClaimsPrincipal(new ClaimsIdentity(new[] {
                new Claim(ClaimTypes.Name,userName)
            },"jwtAuthType"));

            var authState = Task.FromResult(new AuthenticationState(cp));

            NotifyAuthenticationStateChanged(authState);
        }
        public void NotifyUserLogout()
        {
            var authState = Task.FromResult(anonymous);
            NotifyAuthenticationStateChanged(authState);
        }
    }

}
