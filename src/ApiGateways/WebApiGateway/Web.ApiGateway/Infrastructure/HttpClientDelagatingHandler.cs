using System.Net.Http;

namespace Web.ApiGateway.Infrastructure
{
    public class HttpClientDelagatingHandler:DelegatingHandler
    {
        private readonly IHttpContextAccessor _contextAccessor;
        public HttpClientDelagatingHandler(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var authorizationHeader=_contextAccessor.HttpContext.Request.Headers["Authorization"];
            if (!string.IsNullOrEmpty(authorizationHeader))
            {
                if (request.Headers.Contains("Authorization"))
                {
                    request.Headers.Remove("Authorization");
                }
                request.Headers.Add("Authorization", new List<string>() { authorizationHeader});
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}
