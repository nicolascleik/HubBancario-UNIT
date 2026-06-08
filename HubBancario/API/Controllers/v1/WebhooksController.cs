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
        public async Task<IActionResult> ReceivePixWebhook()
        {
            _logger.LogInformation("Webhook recebido na porta de entrada do Hub Bancário.");

            // 1. Extrai o payload bruto (string JSON) enviado pelo banco parceiro
            using var reader = new StreamReader(Request.Body);
            var rawJson = await reader.ReadToEndAsync();

            if (string.IsNullOrWhiteSpace(rawJson))
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

            // 2. Despacha o JSON de forma ultra rápida para a fila 'webhook_events_queue' do RabbitMQ
            // Isso desacopla a API e impede que flutuações de carga derrubem o sistema
            await _messageQueueService.EnqueueWebhookEventAsync(rawJson);

            // 3. Responde imediatamente ao banco. O padrão 202 (Accepted) é o mais correto RESTful 
            // para indicar que a requisição foi aceita para processamento assíncrono.
            return Accepted();
        }
    }
}