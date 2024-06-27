using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace WebApp.Extensions
{
    public static class HttpClientExtension
    {
        public async static Task<TResult>PostGetResponseAysnc<TResult,TValue>(this HttpClient Client,string url,TValue value)
        {
            var httpRes = await Client.PostAsJsonAsync(url, value);
            return httpRes.IsSuccessStatusCode ? await httpRes.Content.ReadFromJsonAsync<TResult>() : default;
        }
        public async static Task PostAsync<TValue>(this HttpClient client,string url,TValue value)
        {
            await client.PostAsJsonAsync(url, value);
        }
        public async static Task<T> GetResponseAsync<T>(this HttpClient client,string url)
        {
            return await client.GetFromJsonAsync<T>(url);
        }
    }
}
