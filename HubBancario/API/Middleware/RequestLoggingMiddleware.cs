using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace HubBancario.API.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var method = context.Request.Method;
            var path = context.Request.Path;
            
            var correlationId = context.Items["CorrelationId"]?.ToString() ?? "UNKNOWN";

            _logger.LogInformation("[{CorrelationId}] Requisição INICIADA em {Method} {Path}", correlationId, method, path);

            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();
                var statusCode = context.Response.StatusCode;

                // Loga a conclusão e os milissegundos gastos (vital para dashboards de performance)
                _logger.LogInformation("[{CorrelationId}] Requisição FINALIZADA em {Method} {Path} com Status {StatusCode} (Duração: {ElapsedMilliseconds}ms)", 
                    correlationId, method, path, statusCode, stopwatch.ElapsedMilliseconds);
            }
        }
    }
}