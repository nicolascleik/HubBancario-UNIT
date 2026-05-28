using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace HubBancario.Infrastructure.Messaging.RabbitMQ
{
    public class RabbitMQConnection : IDisposable
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<RabbitMQConnection> _logger;
        
        private IConnection _connection;
        private readonly object _syncRoot = new object();
        private bool _disposed;

        public RabbitMQConnection(IConfiguration configuration, ILogger<RabbitMQConnection> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public IConnection GetConnection()
        {
            // Abordagem Double-Check Locking para garantir que apenas uma conexão TCP seja criada
            if (_connection != null && _connection.IsOpen)
                return _connection;

            lock (_syncRoot)
            {
                if (_connection != null && _connection.IsOpen)
                    return _connection;

                try
                {
                    var factory = new ConnectionFactory()
                    {
                        HostName = _configuration["RabbitMq:Host"] ?? "localhost",
                        UserName = _configuration["RabbitMq:User"] ?? "guest",
                        Password = _configuration["RabbitMq:Password"] ?? "guest",
                        DispatchConsumersAsync = true // Essencial para alta performance ao consumir filas futuramente
                    };

                    _connection = factory.CreateConnection();
                    
                    // Adiciona um listener para registrar no log caso o RabbitMQ derrube a conexão
                    _connection.ConnectionShutdown += (sender, e) => 
                    {
                        _logger.LogWarning("Conexão com RabbitMQ foi perdida/encerrada. Motivo: {Reason}", e.ReplyText);
                    };

                    _logger.LogInformation("Conexão com RabbitMQ estabelecida com sucesso.");
                    return _connection;
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex, "Falha crítica ao tentar estabelecer conexão com o RabbitMQ.");
                    throw;
                }
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            try
            {
                if (_connection != null && _connection.IsOpen)
                {
                    _connection.Close();
                    _connection.Dispose();
                    _logger.LogInformation("Conexão com RabbitMQ fechada de forma segura (Graceful Shutdown).");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao tentar fechar a conexão com RabbitMQ.");
            }
        }
    }
}