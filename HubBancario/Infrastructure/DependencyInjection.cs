using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Hangfire;
using Hangfire.PostgreSql;
using HubBancario.Application.Interfaces;
using HubBancario.Domain.Repositories;
using HubBancario.Infrastructure.BackgroundJobs;
using HubBancario.Infrastructure.BankAdapters;
using HubBancario.Infrastructure.BankAdapters.Itau;
using HubBancario.Infrastructure.Messaging;
using HubBancario.Infrastructure.Messaging.RabbitMQ;
using HubBancario.Infrastructure.Notifications;
using HubBancario.Infrastructure.Persistence;
using HubBancario.Infrastructure.Persistence.Repositories;

namespace HubBancario.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // 1. Persistência de Dados (PostgreSQL + EF Core)
            services.AddDbContext<BankingDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            // 2. Repositórios e Transações (Unit of Work)
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IPixKeyRepository, PixKeyRepository>();
            services.AddScoped<IClientSecretRepository, ClientSecretRepository>();
            services.AddScoped<IInvoiceRepository, InvoiceRepository>();
            services.AddScoped<IPixChargeRepository, PixChargeRepository>();
            services.AddScoped<IAuditLogRepository, AuditLogRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // 3. Integração Bancária (Adapters)
            // Configura o Options Pattern para ler do appsettings.json
            services.Configure<ItauOptions>(configuration.GetSection(ItauOptions.SectionName));
            
            // AddHttpClient injeta automaticamente o HttpClient gerenciado pelo .NET para evitar socket exhaustion
            services.AddHttpClient<ITokenProvider, ItauTokenProvider>();
            services.AddHttpClient<ItauPixAdapter>();
            services.AddScoped<IBankAdapterFactory, BankAdapterFactory>();

            // 4. Filas e Mensageria (RabbitMQ)
            // A conexão TCP deve ser Singleton (única para toda a vida da aplicação)
            services.AddSingleton<RabbitMQConnection>();
            services.AddScoped<IMessageQueue, RabbitMQPublisher>();
            services.AddScoped<IMessageQueueService, MessageQueueService>();

            // 5. Notificações B2B (Webhooks)
            services.AddHttpClient<INotificationService, WebhookNotificationService>();

            // 6. Background Jobs (Hangfire)
            // Utiliza o mesmo banco de dados PostgreSQL para guardar o estado das tarefas
            services.AddHangfire(config => config
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UsePostgreSqlStorage(options =>
                    options.UseNpgsqlConnection(configuration.GetConnectionString("DefaultConnection"))));

            // Registra o servidor do Hangfire para processar as filas localmente
            services.AddHangfireServer();
            
            // Registra os Jobs no contêiner para que eles possam receber injeções
            services.AddScoped<ReconciliationJob>();
            services.AddScoped<PollingJob>();

            return services;
        }
    }
}