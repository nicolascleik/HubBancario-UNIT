using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HubBancario.API.Filters
{
    public class ValidationFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Verifica se o JSON de entrada foi convertido perfeitamente para os DTOs
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState
                    .Where(x => x.Value != null && x.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                // Padronização rigorosa RFC 7807
                var problemDetails = new ValidationProblemDetails(errors)
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Falha na Validação dos Dados de Entrada",
                    Detail = "Um ou mais campos enviados no corpo da requisição possuem formato inválido ou estão ausentes.",
                    Instance = context.HttpContext.Request.Path
                };

                // Bloqueia a execução e devolve 400 Bad Request imediatamente
                context.Result = new BadRequestObjectResult(problemDetails);
                return;
            }

            // Permite que o fluxo siga para o Controller
            await next();
        }
    }
}