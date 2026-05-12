using MediatR;

namespace HubBancario.Application.UseCases.Payments.ProcessWebhook
{
    /// <summary>
    /// Comando disparado pelo Worker que consome a fila do RabbitMQ.
    /// Contém o payload bruto do webhook enviado pelo banco parceiro (Itaú/Mock).
    /// Este comando faz a baixa do pagamento: muda status da Invoice para Paid,
    /// credita o saldo na ClientAccount e gera AuditLog imutável.
    /// </summary>
    public class ProcessWebhookCommand : IRequest<Unit>
    {
        /// <summary>TxId da cobrança Pix gerada pelo BACEN (identificador único).</summary>
        public string TxId { get; init; }

        /// <summary>Valor efetivamente pago.</summary>
        public decimal PaidAmount { get; init; }

        /// <summary>Payload JSON bruto recebido do banco (para auditoria).</summary>
        public string RawPayload { get; init; }
    }
}
