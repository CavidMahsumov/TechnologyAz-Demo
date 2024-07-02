using Blazored.LocalStorage;
using WebApp.Extensions;

namespace WebApp.Blazor.Infrastructure
{
    public class AuthTokenHandler:DelegatingHandler
    {
        private readonly ISyncLocalStorageService storageService;

        public AuthTokenHandler(ISyncLocalStorageService storageService)
        {
            this.storageService = storageService;
        }
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (storageService != null)
            {
                var token = storageService.GetToken();
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", token);
            }
            return base.SendAsync(request, cancellationToken);
        }
    }
}
