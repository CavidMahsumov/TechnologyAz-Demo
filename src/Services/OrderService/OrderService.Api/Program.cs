using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.Factory;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OrderService.Api.Extensions.Registration.EventHandlerRegistration;
using OrderService.Api.IntegrationEvents.EventHandlers;
using OrderService.Api.IntegrationEvents.Events;
using OrderService.Infrastructure;
using OrderService.Infrastructure.Context;
using System;
using System.Runtime.ConstrainedExecution;
using System.Threading.Tasks;


var builder = WebApplication.CreateBuilder(args);

// Configure the Configuration
builder.Configuration.AddConfiguration(GetConfiguration());

// Register DbContext
builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Set ValidateOnBuild to false
builder.Host.UseDefaultServiceProvider((context, options) =>
{
    options.ValidateOnBuild = false;
});

// Register other services
builder.Services.AddRazorPages(); // Example service registration. Add your services here.
builder.Services.AddSingleton<OrderDbContextSeed>(); // Register the seeder

var app = builder.Build();

// Migrate and seed the database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<OrderDbContext>();
        var logger = services.GetRequiredService<ILogger<OrderDbContext>>();
        var dbContextSeeder = services.GetRequiredService<OrderDbContextSeed>();

        // Migrate the database
        context.Database.Migrate();
        logger.LogInformation("Database migration completed.");

        // Seed the database
        await dbContextSeeder.SeedAsync(context, logger);
        logger.LogInformation("Database seeding completed.");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<OrderDbContext>>();
        logger.LogError(ex, "An error occurred during database migration or seeding.");
    }
}
ConfigureService(builder.Services);

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

ConfigureEventBusForSubscription(app);

app.MapRazorPages(); // Example endpoint mapping. Map your endpoints here.

app.Run();

static IConfiguration GetConfiguration()
{
    return new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .Build();
}

void ConfigureService(IServiceCollection services)
{
    services.AddLogging(config => config.AddConsole())
        .AddApplicationRegistration(typeof(Program))
        .AddPersistenceRegistration(builder.Configuration)
        .ConfigureEventHandlers();

    services.AddSingleton(sp => {
        EventBusConfig config = new()
        {
            ConnectionRetryCount = 5,
            EventNameSuffix = "IntegrationEvent",
            SubscriberClientAppName = "OrderService",
            EventbusType = EventBusConfig.EventBusType.RabbitMQ
        };
        return EventBusFactory.Create(config,sp);   
    
    });
}
void ConfigureEventBusForSubscription(IApplicationBuilder builder)
{
    var eventBus = app.Services.GetRequiredService<IEventBus>();
    eventBus.Subscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();
}
