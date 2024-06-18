using BasketService.Api.Core.Application.Repository;
using BasketService.Api.Core.Application.Services;
using BasketService.Api.Extensions;
using BasketService.Api.Extensions.Registration;
using BasketService.Api.Infrastructure.Repository;
using BasketService.Api.IntegrationEvents.EventHandlers;
using BasketService.Api.IntegrationEvents.Events;
using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.Factory;
using Polly;
using System.Runtime.CompilerServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
ConfigureServicesExt(builder.Services);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();

app.UseAuthorization();

//app.RegisterWithConsul();
ConfigureSubscription(app.Services);

app.MapControllers();

app.Run();


 void ConfigureServicesExt(IServiceCollection services)
 {
    services.ConfigureAuth(builder.Configuration);
    services.AddSingleton(builder.Services.ConfigureRedis(builder.Configuration));

    services.AddHttpContextAccessor();
    services.AddScoped<IBasketRepository, RedisBasketRepository>();
    services.AddTransient<IIdentityService, IdentityService>();
    //services.ConfigureConsul();
    services.AddSingleton<IEventBus>(sp =>
    {
        EventBusConfig config = new()
        {
            ConnectionRetryCount = 5,
            EventNameSuffix = "IntegrationEvent",
            SubscriberClientAppName = "BasketService",
            EventbusType = EventBusConfig.EventBusType.RabbitMQ

        };
        return EventBusFactory.Create(config,sp);  
       
    });
    services.AddTransient<OrderCreatedIntegrationEventHandler>();
 }
 void ConfigureSubscription(IServiceProvider services)
{
    var eventBus=services.GetRequiredService<IEventBus>();
    eventBus.Subscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();
}