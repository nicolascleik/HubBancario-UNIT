using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using HubBancario.Application.Interfaces;
using HubBancario.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace HubBancario.Infrastructure.Notifications
{
    public class WebhookNotificationService : INotificationService
    {
        private readonly HttpClient _httpClient;
        private readonly IAccountRepository _accountRepository;
        private readonly ILogger<WebhookNotificationService> _logger;

        public WebhookNotificationService(
            HttpClient httpClient,
            IAccountRepository accountRepository,
            ILogger<WebhookNotificationService> logger)
        {
            _httpClient = httpClient;
            _accountRepository = accountRepository;
            _logger = logger;
        }

        public async Task NotifyClientAsync(Guid accountId, object payload)
        {
            _logger.LogInformation("Iniciando despacho de notificação Webhook B2B para a Account {AccountId}", accountId);

            // 1. Localiza a conta lojista no banco para extrair suas configurações de comunicação
            var account = await _accountRepository.GetByIdAsync(accountId);
            if (account == null)
            {
                _logger.LogError("Falha crítica no envio do Webhook: A conta {AccountId} não foi encontrada no banco de dados.", accountId);
                return;
            }

            // Fallback de desenvolvimento para o seu ambiente local de testes (ex: apontando pro Wiremock ou API teste)
            string webhookUrl = "http://localhost:8080/api/mock-erp-webhook";

            try
            {
                // Tenta extrair dinamicamente a propriedade WebhookUrl do Agregado Account
                var prop = account.GetType().GetProperty("WebhookUrl");
                if (prop != null)
                {
                    var value = prop.GetValue(account)?.ToString();
                    if (!string.IsNullOrEmpty(value))
                    {
                        webhookUrl = value;
                    }
                }
            }
            catch
            {
                // Mantém o fallback caso a migração da propriedade no Aggregate Root do Domínio ainda não esteja concluída
            }

            if (string.IsNullOrWhiteSpace(webhookUrl))
            {
                _logger.LogWarning("O envio foi abortado pois não há nenhuma URL de Webhook configurada para a conta {AccountId}.", accountId);
                return;
            }

            try
            {
                _logger.LogInformation("Despachando HTTP POST de notificação para: {WebhookUrl}", webhookUrl);

                // Trava de segurança: Se o servidor do lojista não responder em 10 segundos, cancelamos para poupar recursos do Hub
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

                // 2. Realiza o disparo enviando o payload serializado nativamente em JSON
                var response = await _httpClient.PostAsJsonAsync(webhookUrl, payload, cts.Token);

                // 3. Rastreabilidade total da entrega financeira
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Webhook financeiro entregue com sucesso para a conta {AccountId}. HTTP Status: {StatusCode}", 
                        accountId, (int)response.StatusCode);
                }
                else
                {
                    _logger.LogWarning("O ERP do lojista rejeitou o Webhook para a conta {AccountId}. HTTP Status retornado: {StatusCode}", 
                        accountId, (int)response.StatusCode);
                    
                    // Nota de Engenharia: Em uma fase posterior, aqui dispararíamos uma mensagem para a fila de Retry (DLQ) no RabbitMQ
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogError("Timeout estourado (10s) ao tentar notificar o lojista na URL {WebhookUrl} da conta {AccountId}.", webhookUrl, accountId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro de I/O ou falha física de rede ao tentar conectar com a URL {WebhookUrl}.", webhookUrl);
            }
        }
    }
}