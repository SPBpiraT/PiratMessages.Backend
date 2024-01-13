using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PiratMessages.Application.Interfaces;

namespace PiratMessages.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services,
            IConfiguration configuration)
        {
            var connectionString = configuration["DbConnection"];
            services.AddDbContext<MessagesDbContext>(options =>
            {
                options.UseSqlServer(connectionString, b => b.MigrationsAssembly("Migrations"));
            });
            services.AddScoped<IMessagesDbContext>(provider =>
                provider.GetService<MessagesDbContext>());
            return services;
        }
    }
}
