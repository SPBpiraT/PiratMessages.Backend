using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PiratMessages.Application.Common.Caching.Extensions;
using PiratMessages.Caching.Redis.Client;
using PiratMessages.Caching.Redis.Options;

namespace PiratMessages.Caching.Redis.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRedis(this IServiceCollection services)
        {
            services
                .AddOptions<RedisOptions>()
                .Configure<IConfiguration>((x, conf) => conf.GetSection(RedisOptions.Section).Bind(x));

            services.AddCachingClient<RedisClient>();

            return services;
        }
    }
}
