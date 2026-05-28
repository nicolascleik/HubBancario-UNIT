using System;
using System.Text.Json;
using System.Threading.Tasks;
using HubBancario.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HubBancario.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Deixa a requisição seguir o seu fluxo normal
                await _next(context);
            }
            catch (DomainException ex)
            {
                // Erros de Domínio: A culpa é de quem chamou a API (ex: valor negativo)
                _logger.LogWarning(ex, "Exceção de Domínio interceptada: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex, StatusCodes.Status400BadRequest, "Violação de Regra de Negócio");
            }
            catch (Exception ex)
            {
                // Erros Não Tratados: A culpa é do nosso servidor (ex: NullReference, falha de conexão)
                _logger.LogError(ex, "Falha crítica não tratada capturada pelo Middleware.");
                await HandleExceptionAsync(context, ex, StatusCodes.Status500InternalServerError, "Erro Interno do Servidor");
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception, int statusCode, string title)
        {
            context.Response.ContentType = "application/problem+json"; // Formato padrão de mercado
            context.Response.StatusCode = statusCode;

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                // Oculta a mensagem real de erros 500 para evitar vazamento de infraestrutura (Security)
                Detail = statusCode == StatusCodes.Status500InternalServerError 
                    ? "Ocorreu um erro inesperado no servidor. Contate o suporte técnico." 
                    : exception.Message,
                Type = $"https://httpstatuses.com/{statusCode}"
            };

            // Anexa o CorrelationId na resposta de erro para o cliente poder abrir um chamado de suporte
            if (context.Items.TryGetValue("CorrelationId", out var correlationId))
            {
                problemDetails.Extensions["correlationId"] = correlationId;
            }

            var json = JsonSerializer.Serialize(problemDetails);
            return context.Response.WriteAsync(json);
        }
    }
}