using HubBancario.API.Filters;
using Microsoft.AspNetCore.Mvc;

namespace HubBancario.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Adiciona os controllers e registra os filtros globais da API
        services.AddControllers(options =>
        {
            options.Filters.Add<ValidationFilter>();
            options.Filters.Add<IdempotencyFilter>();
        });

        // Desabilita o comportamento padrão de validação para que
        // o ValidationFilter trate os erros de forma centralizada
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
        });

        // Registro dos filtros no container de injeção de dependência
        services.AddScoped<ValidationFilter>();
        services.AddScoped<IdempotencyFilter>();

        // Habilita a geração da documentação OpenAPI
        services.AddOpenApi();

        // Política de CORS utilizada pela aplicação
        services.AddCors(options =>
        {
            options.AddPolicy("DefaultCorsPolicy", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
        });

        // Cliente HTTP para chamadas a APIs externas
        services.AddHttpClient();

        return services;
    }
}