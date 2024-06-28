using CatalogService.Api.Extensions;
using CatalogService.Api.Infrastructure;
using CatalogService.Api.Infrastructure.Context;
using Consul;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Reflection;

var options = new WebApplicationOptions
{
    Args = args,
    ContentRootPath = Directory.GetCurrentDirectory(),
    WebRootPath = "Pics"
};
var builder = WebApplication.CreateBuilder(options);
builder.WebHost.UseWebRoot("Pics");
builder.WebHost.UseContentRoot(Directory.GetCurrentDirectory());

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

// Add services to the container.
builder.Services.Configure<CatalogSettings>(builder.Configuration.GetSection(nameof(CatalogSettings)));

// Register CatalogSettings to use directly
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<CatalogSettings>>().Value);

// Configure the DbContext
builder.Services.ConfigureDbContext(builder.Configuration);
//builder.Services.ConfigureConsul(builder.Configuration);

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();
app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(System.IO.Path.Combine(app.Environment.ContentRootPath,"Pics")),
    RequestPath="/pics"
    
});



// Apply database migrations and seed data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<CatalogContext>();

        var env = services.GetRequiredService<IWebHostEnvironment>();
        var logger = services.GetRequiredService<ILogger<CatalogContextSeed>>();

        // Apply database migrations
        context.Database.Migrate();

        // Seed the database with initial data
        new CatalogContextSeed().SeedAsync(context, env, logger).Wait();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating or seeding the database.");
    }
}
var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();

//app.RegisterWithConsul(lifetime);
app.Run();
