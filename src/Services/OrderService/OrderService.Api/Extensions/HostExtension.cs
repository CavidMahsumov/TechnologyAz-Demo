using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Polly;

namespace OrderService.Api.Extensions
{
    public static class HostExtension
    {
        public static IWebHost MigrateDbContext<TContext>(IWebHost host,Action<TContext,IServiceProvider>seeder) where TContext:DbContext
        {
            using (var scope=host.Services.CreateScope())
            {
                var services=scope.ServiceProvider;

                var logger=services.GetRequiredService<ILogger<TContext>>();

                var context = services.GetService<TContext>();
                try
                {
                    logger.LogInformation("Migrating Database associated with context {DbContextName}", typeof(TContext).Name);

                    var retry = Policy.Handle<SqlException>()
                        .WaitAndRetry(new TimeSpan[] {
                            TimeSpan.FromSeconds(3),
                            TimeSpan.FromSeconds(5),
                            TimeSpan.FromSeconds(8),

                        });
                    retry.Execute(() => InvokeSeeder(seeder, context, services));

                    logger.LogInformation("Migrated database");

                }
                catch (Exception)
                {
                    logger.LogError("An error ocurred while migration the database used on context{DbContextName}",typeof(TContext).Name);
                }
            }
            return host;
        }

        private static void InvokeSeeder<TContext>(Action<TContext,IServiceProvider>seeder,TContext context,IServiceProvider services) where TContext:DbContext
        {
            context.Database.EnsureCreated();//check db is created or no
            context.Database.Migrate();

            seeder(context, services);
        }
    }
}
