using Blazored.LocalStorage;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace WebApp.Extensions
{
    public static class HttpClientExtension
    {
        private readonly static ISyncLocalStorageService _syncLocalStorageService;


        public async static Task<TResult>PostGetResponseAysnc<TResult,TValue>(this HttpClient Client,string url,TValue value)
        {
            var httpRes = await Client.PostAsJsonAsync(url, value);
            return httpRes.IsSuccessStatusCode ? await httpRes.Content.ReadFromJsonAsync<TResult>() : default;
        }
        public async static Task PostAsync<TValue>(this HttpClient client,string url,TValue value,string token)
        {
            //await client.PostAsJsonAsync(url, value);
            await client.PostAsJsonAsync("http://localhost:5000/Basket/AddBasketItemAsync", value);
            //var requestMessage = new HttpRequestMessage(HttpMethod.Post, url)
            //{
            //    Content = JsonContent.Create(value)
            //};
            //// Yetkilendirme başlığını ekle
            //requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer",token);
            
            //// İsteği gönder
            //var response = await client.SendAsync(requestMessage);

            //// Hata kontrolü (isteğe bağlı)
            //response.EnsureSuccessStatusCode();
        }
        public async static Task<T> GetResponseAsync<T>(this HttpClient client,string url)
        {
            return await client.GetFromJsonAsync<T>(url);
        }
    }
}
