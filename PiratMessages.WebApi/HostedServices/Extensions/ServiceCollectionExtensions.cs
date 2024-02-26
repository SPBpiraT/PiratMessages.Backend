namespace PiratMessages.WebApi.HostedServices.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationHostedServices(this IServiceCollection services)
        {
            // services.AddHostedService<DbInitHostedService>();

            services.AddHostedService<RedisTestHostedService>();

            // services.AddHostedService<RabbitMQTestHostedService>();

            return services;
        }
    }
}
