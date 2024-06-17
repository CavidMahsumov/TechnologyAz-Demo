using StackExchange.Redis;

namespace BasketService.Api.Extensions
{
    public static  class RedisRegistration
    {
        public static ConnectionMultiplexer ConfigureRedis(this IServiceCollection services,IConfiguration config)
        {
            var redisConf = ConfigurationOptions.Parse(config["RedisSettings:ConnectionString"], true);
            redisConf.ResolveDns = true;

            return ConnectionMultiplexer.Connect(redisConf);
        }
    }
}
