using System.Threading.Tasks;
using HubBancario.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace HubBancario.Infrastructure.Messaging
{
    // Se quiser, você pode criar uma interface IMessageQueueService na Application,
    // mas muitas vezes ele próprio implementa IMessageQueue e repassa para o provedor específico (RabbitMQ).
    // Aqui vamos usá-lo como um serviço de conveniência focado nos casos de uso do Hub.
    
    public interface IMessageQueueService
    {
        Task EnqueueWebhookEventAsync<T>(T payload);
        Task EnqueueReconciliationTaskAsync<T>(T payload);
    }

    public class MessageQueueService : IMessageQueueService
    {
        private readonly IMessageQueue _messageQueue;
        private readonly ILogger<MessageQueueService> _logger;

        // Centralização de "Magic Strings" (Nomes das filas)
        private const string WebhookQueueName = "webhook_events_queue";
        private const string ReconciliationQueueName = "reconciliation_tasks_queue";

        public MessageQueueService(IMessageQueue messageQueue, ILogger<MessageQueueService> logger)
        {
            _messageQueue = messageQueue;
            _logger = logger;
        }

        public async Task EnqueueWebhookEventAsync<T>(T payload)
        {
            _logger.LogInformation("Enfileirando evento de webhook para a fila {QueueName}", WebhookQueueName);
            
            // Aqui poderíamos adicionar metadados padrão à mensagem se necessário
            
            await _messageQueue.PublishAsync(payload, WebhookQueueName);
        }

        public async Task EnqueueReconciliationTaskAsync<T>(T payload)
        {
            _logger.LogInformation("Enfileirando tarefa de conciliação para a fila {QueueName}", ReconciliationQueueName);
            
            await _messageQueue.PublishAsync(payload, ReconciliationQueueName);
        }
    }
}