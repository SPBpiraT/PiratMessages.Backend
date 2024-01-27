using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PiratMessages.Application.Interfaces;

namespace PiratMessages.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services)
        {
            var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
            var dbName = Environment.GetEnvironmentVariable("DB_NAME");
            var dbPassword = Environment.GetEnvironmentVariable("DB_SA_PASSWORD");

            var connectionString = $"Data Source={dbHost};Initial Catalog={dbName};User ID=sa;Password={dbPassword}";

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
