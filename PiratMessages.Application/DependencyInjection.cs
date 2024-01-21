using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using PiratMessages.Application.Common.Behaviors;
using System.Reflection;

namespace PiratMessages.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));

            services.AddValidatorsFromAssemblies(new[] { Assembly.GetExecutingAssembly() });

            services.AddTransient(typeof(IPipelineBehavior<,>),
                typeof(ValidationBehavior<,>));

            services.AddTransient(typeof(IPipelineBehavior<,>),
                typeof(LoggingBehavior<,>));

            return services;
        }
    }
}
