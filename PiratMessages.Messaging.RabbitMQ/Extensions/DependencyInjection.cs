using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using PiratMessages.Messaging.RabbitMQ.Options;
using PiratMessages.Application.Common.Messaging.Extensions;

namespace PiratMessages.Messaging.RabbitMQ.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddRabbitMQ(this IServiceCollection services)
        {
            services
                .AddOptions<RabbitMQOptions>()
                .Configure<IConfiguration>((x, conf) => conf.GetSection(RabbitMQOptions.Section).Bind(x));

            services.AddMessaging<RabbitMQClient>();

            return services;
        }
    }
}