using System;
using System.Threading;
using System.Threading.Tasks;
using HubBancario.Application.Commands.ProcessWebhook;
using HubBancario.Application.Interfaces;
using HubBancario.Domain.Aggregates.PixCharge;
using HubBancario.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HubBancario.Application.Services
{
    /// <summary>
    /// Serviço de background encarregado da Conciliação Bancária Ativa (Rede de segurança para falhas de Webhook).
    /// </summary>
    public class PixReconciliationService : BackgroundService
    {
        private readonly IPixChargeRepository _pixChargeRepository;
        private readonly IBankPixAdapter _bankPixAdapter;
        private readonly IMediator _mediator;
        private readonly ILogger<PixReconciliationService> _logger;

        public PixReconciliationService(
            IPixChargeRepository pixChargeRepository,
            IBankPixAdapter bankPixAdapter,
            IMediator mediator,
            ILogger<PixReconciliationService> logger)
        {
            _pixChargeRepository = pixChargeRepository;
            _bankPixAdapter = bankPixAdapter;
            _mediator = mediator;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Serviço de Conciliação Ativa Pix inicializado com sucesso.");

            // Loop de execução em segundo plano enquanto o ecossistema estiver ativo
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Iniciando varredura de rotina para conciliação de cobranças pendentes...");

                    // NOTA DE IMPLEMENTAÇÃO: Para viabilizar este método perfeitamente, certifique-se de expor 
                    // uma listagem de itens ativos (ex: GetActiveChargesAsync) na sua interface IPixChargeRepository 
                    // ou criar uma Query dedicada via MediatR (ex: GetActivePixChargesQuery).
                    
                    /*
                    var pendingCharges = await _pixChargeRepository.GetActiveChargesAsync();

                    foreach (var charge in pendingCharges)
                    {
                        _logger.LogInformation("Verificando status da transação ativa TxId: {TxId} no banco parceiro.", charge.TxId.Value);
                        
                        // Consulta ativa (GET) diretamente na API do banco através do adaptador mTLS
                        var currentBankStatus = await _bankPixAdapter.CheckStatusAsync(charge.TxId);

                        // Se o banco confirmar que foi pago, força a baixa de forma segura disparando o comando interno
                        if (currentBankStatus == "CONCLUIDO" || currentBankStatus == "PAID")
                        {
                            _logger.LogWarning("Divergência detectada via Polling! Cobrança TxId: {TxId} consta como PAGA no banco. Forçando reconciliação...", charge.TxId.Value);
                            
                            var webhookCommand = new ProcessWebhookCommand(charge.TxId, "{\"status\": \"PAID\", \"reconciledBy\": \"PollingJob\"}");
                            await _mediator.Send(webhookCommand, stoppingToken);
                        }
                    }
                    */

                    // Configuração de intervalo de execução (Exemplo: Executa a rotina a cada 5 minutos)
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogWarning("O ciclo de conciliação ativa foi interrompido devido ao encerramento da aplicação.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ocorreu uma falha crítica ao executar a rotina de conciliação bancária de Pix.");
                }
            }
        }
    }
}