using Microsoft.Extensions.DependencyInjection;
using PiratMessages.Application.Common.Messaging.Options;
using PiratMessages.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace PiratMessages.Application.Common.Messaging.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMessaging<TClient>(this IServiceCollection services)
            where TClient : class, IMessagingClient
        {
            services
                .AddOptions<MessagingOptions>()
                .Configure<IConfiguration>((x, conf) => conf.GetSection(MessagingOptions.Section).Bind(x));

            services.AddSingleton<IMessagingClient, TClient>();

            return services;
        }
    }
}
