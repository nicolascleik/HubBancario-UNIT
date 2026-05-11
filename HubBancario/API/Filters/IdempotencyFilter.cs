using Microsoft.AspNetCore.Mvc.Filters;

namespace HubBancario.API.Filters;

public class IdempotencyFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {

    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
}