using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;
using Ocelot.Values;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOcelot();/*.AddConsul();*/
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("CorsPolicy",
       builder => builder.SetIsOriginAllowed((host) => true)
       .AllowAnyMethod()
       .AllowAnyHeader()
       .AllowCredentials());
});

builder.Configuration.AddJsonFile("Configurations/ocelot.json");


var app = builder.Build();
app.UseCors("CorsPolicy");
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
