using System.IO;
using System.Threading.Tasks;
using HubBancario.Infrastructure.Messaging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HubBancario.API.Controllers.v1
{
    [ApiController]
    [Route("api/v1/webhooks")]
    [Produces("application/json")]
    public class WebhooksController : ControllerBase
    {
        private readonly IMessageQueueService _messageQueueService;
        private readonly ILogger<WebhooksController> _logger;

        public WebhooksController(IMessageQueueService messageQueueService, ILogger<WebhooksController> logger)
        {
            _messageQueueService = messageQueueService;
            _logger = logger;
        }

        /// <summary>
        /// Endpoint exclusivo para recepção de notificações de pagamento Pix (Webhooks) enviadas pelos bancos parceiros.
        /// </summary>
        /// <remarks>
        /// Este endpoint possui alta disponibilidade e vazão. Ele apenas captura o payload bruto,
        /// joga de forma assíncrona na fila do RabbitMQ e libera o canal do banco imediatamente com um status 202 Accepted.
        /// </remarks>
        [HttpPost("pix")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ReceivePixWebhook([FromBody] System.Text.Json.JsonElement payload)
        {
            _logger.LogInformation("Webhook recebido na porta de entrada do Hub Bancário.");

            var rawJson = payload.GetRawText();

            if (string.IsNullOrWhiteSpace(rawJson) || rawJson == "{}")
            {
                _logger.LogWarning("Requisição de Webhook rejeitada: O corpo (body) enviado estava vazio.");
                return BadRequest(new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Payload Ausente",
                    Detail = "Não é possível processar um webhook com o corpo da requisição vazio."
                });
            }

            _logger.LogDebug("Payload bruto capturado com sucesso. Tamanho: {Length} caracteres.", rawJson.Length);

            await _messageQueueService.EnqueueWebhookEventAsync(rawJson);

            return Accepted();
        }
    }
}