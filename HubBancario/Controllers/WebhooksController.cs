using Microsoft.AspNetCore.Mvc;

namespace HubBancario.Controllers;

[ApiController]
[Route("api/webhooks")]
public class WebhooksController : ControllerBase
{
    [HttpPost("{bankId}")]
    public IActionResult ReceberWebhook(string bankId, [FromBody] object body)
    {
        return Ok(new
        {
            mensagem = "Webhook recebido.",
            banco = bankId,
            payload = body
        });
    }
}
