using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace HubBancario.Infrastructure.BackgroundJobs
{
    public class PollingJob
    {
        private readonly ILogger<PollingJob> _logger;

        public PollingJob(ILogger<PollingJob> logger)
        {
            _logger = logger;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Executando varredura genérica (PollingJob) para sistemas síncronos/legados...");

            try
            {
                // Este é um esqueleto para implementações futuras (ex: processamento de Arquivos CNAB, 
                // varredura de e-mails, ou varredura de "Dead Letter Queues" do RabbitMQ).
                
                // Simulação de tempo de I/O
                await Task.Delay(500, cancellationToken);
                
                _logger.LogInformation("Varredura de rotinas síncronas concluída sem anomalias.");
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("O PollingJob foi interrompido (cancellation token acionado pelo desligamento do servidor).");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha crítica durante a execução da varredura genérica.");
            }
        }
    }
}