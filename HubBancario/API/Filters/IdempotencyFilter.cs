using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace HubBancario.API.Filters
{
    public class IdempotencyFilter : IAsyncActionFilter
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<IdempotencyFilter> _logger;
        private const string IdempotencyHeader = "Idempotency-Key";

        public IdempotencyFilter(IDistributedCache cache, ILogger<IdempotencyFilter> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var request = context.HttpContext.Request;

            // A idempotência é crítica nas operações de criação e alteração (POST, PUT, PATCH)
            if (request.Method == HttpMethods.Post || request.Method == HttpMethods.Put || request.Method == HttpMethods.Patch)
            {
                // 1. Exige que o ERP parceiro envie o cabeçalho de proteção
                if (!request.Headers.TryGetValue(IdempotencyHeader, out var idempotencyKey) || string.IsNullOrWhiteSpace(idempotencyKey))
                {
                    _logger.LogWarning("Requisição bloqueada por ausência do cabeçalho de idempotência.");
                    
                    var problemDetails = new ProblemDetails
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Title = "Cabeçalho de Idempotência Ausente",
                        Detail = $"O cabeçalho '{IdempotencyHeader}' é estritamente obrigatório para efetuar operações de mutação de estado na API."
                    };
                    
                    context.Result = new BadRequestObjectResult(problemDetails);
                    return;
                }

                var cacheKey = $"idempotency_key_{idempotencyKey}";
                
                // 2. Verifica se esta chave já foi processada recentemente
                var isAlreadyProcessed = await _cache.GetStringAsync(cacheKey);

                if (!string.IsNullOrEmpty(isAlreadyProcessed))
                {
                    _logger.LogWarning("Conflito de Idempotência detectado para a chave: {IdempotencyKey}", idempotencyKey.ToString());
                    
                    var problemDetails = new ProblemDetails
                    {
                        Status = StatusCodes.Status409Conflict,
                        Title = "Requisição Duplicada Detectada",
                        Detail = $"A chave de idempotência '{idempotencyKey}' fornecida já foi processada recentemente. O sistema ignorou o reprocessamento para evitar transações duplicadas."
                    };
                    
                    context.Result = new ConflictObjectResult(problemDetails);
                    return;
                }

                // 3. Marca a chave como "utilizada" no cofre distribuído com duração de 24 horas
                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
                };
                
                await _cache.SetStringAsync(cacheKey, "processed", cacheOptions);
            }

            // Permite o fluxo normal se a requisição for segura e inédita
            await next();
        }
    }
}