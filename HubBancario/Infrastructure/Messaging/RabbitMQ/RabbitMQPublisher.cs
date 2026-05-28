using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using HubBancario.Application.Interfaces;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace HubBancario.Infrastructure.Messaging.RabbitMQ
{
    public class RabbitMQPublisher : IMessageQueue
    {
        private readonly RabbitMQConnection _connection;
        private readonly ILogger<RabbitMQPublisher> _logger;

        public RabbitMQPublisher(RabbitMQConnection connection, ILogger<RabbitMQPublisher> logger)
        {
            _connection = connection;
            _logger = logger;
        }

        public Task PublishAsync<T>(T message, string queueName)
        {
            try
            {
                var connection = _connection.GetConnection();
                
                // O canal (channel) tem tempo de vida curto e deve ser descartado (using) após a publicação
                using var channel = connection.CreateModel();

                // durable: true garante que a fila em si sobreviva a reinicializações do servidor RabbitMQ
                channel.QueueDeclare(queue: queueName,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var json = JsonSerializer.Serialize(message);
                var body = Encoding.UTF8.GetBytes(json);

                // Persistent = true obriga o RabbitMQ a salvar esta mensagem específica no disco rígido
                var properties = channel.CreateBasicProperties();
                properties.Persistent = true; 

                channel.BasicPublish(exchange: "",
                                     routingKey: queueName,
                                     basicProperties: properties,
                                     body: body);

                _logger.LogDebug("Mensagem enviada com sucesso para a fila {QueueName}.", queueName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha ao tentar publicar mensagem na fila {QueueName}.", queueName);
                throw;
            }

            return Task.CompletedTask;
        }
    }
}