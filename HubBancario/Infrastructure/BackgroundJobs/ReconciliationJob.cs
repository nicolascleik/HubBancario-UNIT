using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HubBancario.Application.Commands.ProcessWebhook;
using HubBancario.Application.Interfaces;
using HubBancario.Domain.Aggregates.PixCharge;
using HubBancario.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HubBancario.Infrastructure.BackgroundJobs
{
    public class ReconciliationJob
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ReconciliationJob> _logger;

        public ReconciliationJob(IServiceProvider serviceProvider, ILogger<ReconciliationJob> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        // Método que o Hangfire vai acionar a cada X minutos
        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Iniciando Job de Conciliação Ativa de Cobranças Pix...");

            // Cria um escopo isolado para garantir que o DbContext seja destruído (Disposed) ao final
            using var scope = _serviceProvider.CreateScope();
            
            var dbContext = scope.ServiceProvider.GetRequiredService<BankingDbContext>();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var bankAdapterFactory = scope.ServiceProvider.GetRequiredService<IBankAdapterFactory>();

            // Busca cobranças ativas. Na vida real, filtramos para ignorar as criadas há menos de 5 minutos,
            // dando tempo para o Webhook natural chegar antes de gastar recursos de processamento.
            var cobrancasPendentes = await dbContext.PixCharges
                .Where(p => p.Status == PixChargeStatus.Active)
                .Take(100) // Processa em lotes para não travar a memória do servidor
                .ToListAsync(cancellationToken);

            if (!cobrancasPendentes.Any())
            {
                _logger.LogInformation("Conciliação finalizada: Nenhuma cobrança Pix pendente encontrada.");
                return;
            }

            foreach (var charge in cobrancasPendentes)
            {
                try
                {
                    // Descobre a qual banco essa cobrança pertence (através da Conta -> Fatura)
                    var invoice = await dbContext.Invoices.FindAsync(new object[] { charge.InvoiceId }, cancellationToken);
                    var account = await dbContext.Accounts.FindAsync(new object[] { invoice.AccountId }, cancellationToken);

                    // Pega o adaptador específico (ex: Itaú)
                    var adapter = bankAdapterFactory.GetAdapter(account.BankId);
                    
                    // Faz a consulta ativa na API do Banco (O nosso GET HTTP)
                    var statusNoBanco = await adapter.CheckStatusAsync(charge.TxId.Value);

                    if (statusNoBanco == "PAID")
                    {
                        _logger.LogWarning("Divergência Encontrada: O TxId {TxId} está pendente no Hub, mas consta como PAGO no Banco. Forçando conciliação...", charge.TxId.Value);

                        // Reaproveitamos o fluxo do MediatR que já criamos! 
                        // Isso garante que os Logs de Auditoria e Webhooks B2B rodem perfeitamente.
                        var command = new ProcessWebhookCommand
                        {
                            TxId = charge.TxId.Value,
                            RawPayload = "{\"source\": \"Conciliacao_BackgroundJob\", \"motivo\": \"Webhook Nao Recebido/Perdido\"}"
                        };

                        await mediator.Send(command, cancellationToken);
                    }
                    else if (statusNoBanco == "EXPIRED")
                    {
                        _logger.LogInformation("O TxId {TxId} expirou no banco parceiro. (Implementar fluxo de baixa por expiração futuramente).", charge.TxId.Value);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro inesperado ao tentar conciliar o TxId {TxId}.", charge.TxId.Value);
                    // O try/catch interno garante que se uma cobrança der erro na rede, o Job continue avaliando as outras 99.
                }
            }

            _logger.LogInformation("Job de Conciliação Ativa concluído com sucesso.");
        }
    }
}