using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WebApp;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using WebApp.Utils;
using WebApp.Application.Services.Interfaces;
using WebApp.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System;
using WebApp.Blazor;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });


builder.Services.AddBlazoredLocalStorage();

builder.Services.AddTransient<IIdentityService, WebApp.Application.Services.IdentityService>();
builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();
builder.Services.AddScoped(sp =>
{
    var clientFactor = sp.GetRequiredService<IHttpClientFactory>();
    return clientFactor.CreateClient("ApiGatewayHttpClient");
});
builder.Services.AddHttpClient("ApiGatewayHttpClient", client => {
    client.BaseAddress = new Uri("http://localhost:5000/");
});

await builder.Build().RunAsync();
