using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PiratMessages.Application.Common.Caching.Options;
using PiratMessages.Application.Interfaces;

namespace PiratMessages.Application.Common.Caching.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCachingClient<TClient>(this IServiceCollection services)
            where TClient : class, ICachingClient
        {
            services
                .AddOptions<CachingClientOptions>()
                .Configure<IConfiguration>((x, conf) => conf.GetSection(CachingClientOptions.Section).Bind(x));

            services.AddSingleton<ICachingClient, TClient>();

            return services;
        }
    }
}
