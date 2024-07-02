using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;
using Ocelot.Values;
using Web.ApiGateway.Infrastructure;
using Web.ApiGateway.Services;
using Web.ApiGateway.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ICatalogService, CatalogService>();
builder.Services.AddScoped<IBasketService, BasketService>();

builder.Services.AddOcelot();/*.AddConsul();*/
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("CorsPolicy",
       builder => builder.SetIsOriginAllowed((host) => true)
       .AllowAnyMethod()
       .AllowAnyHeader()
       .AllowCredentials());
});
ConfigureHttpClient(builder.Services);

builder.Configuration.AddJsonFile("Configurations/ocelot.json");


var app = builder.Build();
app.UseCors("CorsPolicy");
app.UseCors("AllowLocalhost");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

//app.UseAuthorization();

app.MapControllers();
app.UseOcelot().Wait();

app.Run();

void ConfigureHttpClient(IServiceCollection services){
    services.AddCors(options =>
    {
        options.AddPolicy("AllowLocalhost",
            builder =>
            {
                builder.WithOrigins("http://localhost:2000")
                       .AllowAnyHeader()
                       .AllowAnyMethod();
            });
    });
    services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
    services.AddHttpClient("basket", c =>
    {
        c.BaseAddress = new Uri(builder.Configuration["urls:basket"]);
    }).AddHttpMessageHandler<HttpClientDelagatingHandler>();
    services.AddHttpClient("catalog", c =>
    {
        c.BaseAddress = new Uri(builder.Configuration["urls:catalog"]);
    }).AddHttpMessageHandler<HttpClientDelagatingHandler>();

}
