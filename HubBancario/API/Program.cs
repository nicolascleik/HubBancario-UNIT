using HubBancario.API.Extensions;
using HubBancario.API.Middleware;
using HubBancario.Infrastructure.Persistence; 
using HubBancario.Infrastructure.Messaging.RabbitMQ;
using Microsoft.EntityFrameworkCore; 

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiServices(builder.Configuration);
builder.Services.AddHostedService<WebhookConsumerWorker>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<BankingDbContext>();
        context.Database.EnsureCreated();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro ao inicializar o banco de dados: {ex.Message}");
    }
}

app.UseMiddleware<ExceptionMiddleware>();

app.UseMiddleware<CorrelationIdMiddleware>();

app.UseMiddleware<RequestLoggingMiddleware>();

app.ConfigurePipeline();

app.Run();
