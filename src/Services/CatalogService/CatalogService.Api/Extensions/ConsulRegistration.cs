using Consul;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using System.Runtime.CompilerServices;

namespace CatalogService.Api.Extensions
{
    public static class ConsulRegistration
    {
        public static IServiceCollection ConfigureConsul(this IServiceCollection services,IConfiguration config)
        {
            services.AddSingleton<IConsulClient, ConsulClient>(p=>new ConsulClient(consulconfig =>
            {
                var adress = config["ConsulConfig:Address"];
                consulconfig.Address = new Uri(adress);
            }));
            return services;
        }
        public static IApplicationBuilder RegisterWithConsul(this IApplicationBuilder app,IHostApplicationLifetime lifetime)
            {
                var consulClient=app.ApplicationServices.GetRequiredService<IConsulClient>();
                var loggingFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();

                 var logger=loggingFactory.CreateLogger<IApplicationBuilder>();

            //get server ip adress

            //var features = app.Properties["server.Features"] as FeatureCollection;
            //var addresses = features.Get<IServerAddressesFeature>();
            //var address = addresses.Addresses.FirstOrDefault();

            //////Register service with consul

            //var uri = new Uri(address);
            var registration = new AgentServiceRegistration()
                {
                    ID = $"CatalogService",
                    Name = "CatalogService",
                    Address = "localhost",
                    Port = 5004,
                    Tags = new[] {"Catalog Service","Catalog"}
                };
                logger.LogInformation("Registering with Consul");
                consulClient.Agent.ServiceDeregister(registration.ID).Wait();
                consulClient.Agent.ServiceRegister(registration).Wait();

                lifetime.ApplicationStopping.Register(() =>
                {
                    logger.LogInformation("Deregistering from Consul");
                    consulClient.Agent.ServiceDeregister(registration.ID).Wait();
                });

                return app;
        }
    }
}
