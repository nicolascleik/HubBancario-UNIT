using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace HubBancario.API.Middleware
{
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;
        private const string CorrelationIdHeader = "X-Correlation-ID";

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string correlationId;

            // Se o ERP do cliente já mandar um ID de rastreio, nós o aproveitamos
            if (context.Request.Headers.TryGetValue(CorrelationIdHeader, out StringValues correlationIdValues))
            {
                correlationId = correlationIdValues.ToString();
            }
            else
            {
                // Caso contrário, geramos o nosso próprio (ex: REQ-F3A1B2C4)
                correlationId = $"REQ-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
            }

            // Armazena no escopo da requisição para que os outros Middlewares e Services possam ler
            context.Items["CorrelationId"] = correlationId;

            // Adiciona o CorrelationId de volta no Header da resposta para o cliente saber o protocolo dele
            context.Response.OnStarting(() =>
            {
                if (!context.Response.Headers.ContainsKey(CorrelationIdHeader))
                {
                    context.Response.Headers.Append(CorrelationIdHeader, correlationId);
                }
                return Task.CompletedTask;
            });

            // *Nota de Arquitetura: Em produção, se o Serilog LogContext estiver habilitado,
            // faríamos um PushProperty("CorrelationId", correlationId) aqui.
            
            await _next(context);
        }
    }
}