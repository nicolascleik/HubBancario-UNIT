using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace HubBancario.Application
{
    public static class DependencyInjection
        {
            public static IServiceCollection AddApplication(this IServiceCollection services)
            {
              var assembly = Assembly.GetExecutingAssembly();

             // Regista o MediatR (que vai varrer a Application à procura dos seus Handlers)
             services.AddMediatR(cfg => {
                cfg.RegisterServicesFromAssembly(assembly);
            });

            // Regista os validadores do FluentValidation
            services.AddValidatorsFromAssembly(assembly);

            services.AddAutoMapper(assembly);

            return services;
            }
        }
}