using IdentityService.Api.Application.Services;
using IdentityService.Api.Extensions.Registration;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Hosting.Server;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IIdentityService, IdentityService.Api.Application.Services.IdentityService>();
builder.Services.ConfigureConsul(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//var server = app.Services.GetService<IServer>();
//var addressFeature = server.Features.Get<IServerAddressesFeature>();

//foreach (var address in addressFeature.Addresses)
//{
//    Console.WriteLine("Kestrel is listening on address: " + address);
//}





app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();

app.RegisterWithConsul(lifetime);

app.Run();
