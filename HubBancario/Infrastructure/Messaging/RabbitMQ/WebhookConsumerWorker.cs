using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using HubBancario.Application.Commands.ProcessWebhook;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace HubBancario.Infrastructure.Messaging.RabbitMQ
{
    public class WebhookConsumerWorker : BackgroundService
    {
        private readonly ILogger<WebhookConsumerWorker> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConfiguration _configuration;
        private IConnection _connection;
        private IModel _channel;
        private const string QueueName = "webhook_events_queue";

        public WebhookConsumerWorker(
            ILogger<WebhookConsumerWorker> logger,
            IServiceScopeFactory scopeFactory,
            IConfiguration configuration)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _configuration = configuration;
            
            InitializeRabbitMQ();
        }

        private void InitializeRabbitMQ()
        {
            var factory = new ConnectionFactory
            {
                HostName = _configuration["RabbitMq:Host"] ?? "localhost",
                Port = int.Parse(_configuration["RabbitMq:Port"] ?? "5672"),
                UserName = _configuration["RabbitMq:User"] ?? "guest",
                Password = _configuration["RabbitMq:Password"] ?? "guest",
                DispatchConsumersAsync = true
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var rawMessage = Encoding.UTF8.GetString(body);

                _logger.LogInformation("Mensagem capturada pelo Worker. Desembrulhando payload...");

                try
                {
                    string rawJson = rawMessage;
                    if (rawMessage.StartsWith("\""))
                    {
                        rawJson = JsonSerializer.Deserialize<string>(rawMessage);
                    }

                    using var scope = _scopeFactory.CreateScope();
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                    using var jsonDoc = JsonDocument.Parse(rawJson);
                    var pixArray = jsonDoc.RootElement.GetProperty("pix");
                    var txId = pixArray[0].GetProperty("txid").GetString();

                    var command = new ProcessWebhookCommand 
                    { 
                        TxId = txId, 
                        RawPayload = rawJson 
                    };

                    await mediator.Send(command);

                    _channel.BasicAck(ea.DeliveryTag, multiple: false);
                    
                    _logger.LogInformation("Sucesso! Pix {TxId} compensado. Fatura atualizada para 'Paid'.", txId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Falha crítica ao processar o Webhook. Mensagem descartada para evitar loop.");
                    _channel.BasicNack(ea.DeliveryTag, multiple: false, requeue: false);
                }
            };

            _channel.BasicConsume(queue: QueueName, autoAck: false, consumer: consumer);

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            if (_channel != null && _channel.IsOpen) _channel.Close();
            if (_connection != null && _connection.IsOpen) _connection.Close();
            base.Dispose();
        }
    }
}