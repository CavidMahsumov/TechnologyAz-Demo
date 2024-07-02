using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WebApp;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using WebApp.Blazor.Utils;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System;
using WebApp.Blazor;
using WebApp.Blazor.Application.Services.Interfaces;
using WebApp.Blazor.Application.Services;
using WebApp.Blazor.Infrastructure;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");



builder.Services.AddBlazoredLocalStorage();

builder.Services.AddScoped<IIdentityService, IdentityService>();
builder.Services.AddScoped<ICatalogService,CatalogService>();
builder.Services.AddScoped<IBasketService, BasketService>();
builder.Services.AddTransient<AuthTokenHandler>();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });


builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();
builder.Services.AddScoped(sp =>
{
    var clientFactor = sp.GetRequiredService<IHttpClientFactory>();
    return clientFactor.CreateClient("ApiGatewayHttpClient");
});
builder.Services.AddHttpClient("ApiGatewayHttpClient", client => {
    client.BaseAddress = new Uri("http://localhost:5000/");
});
builder.Services.AddHttpClient("basket", c =>
{
    c.BaseAddress = new Uri(builder.Configuration["http://localhost:5003/api/basket/"]);
}).AddHttpMessageHandler<AuthTokenHandler>();



await builder.Build().RunAsync();
