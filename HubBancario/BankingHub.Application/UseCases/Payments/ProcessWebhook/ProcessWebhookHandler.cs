using System.Threading;
using System.Threading.Tasks;
using HubBancario.Domain.Aggregates.Audit;
using HubBancario.Domain.Aggregates.PixCharge;
using HubBancario.Domain.Exceptions;
using HubBancario.Domain.Repositories;
using HubBancario.Domain.ValueObjects;
using MediatR;

namespace HubBancario.Application.UseCases.Payments.ProcessWebhook
{
    /// <summary>
    /// Handler do comando ProcessWebhookCommand.
    /// É o ponto central da baixa de pagamento:
    ///   1. Localiza o PixCharge pelo TxId.
    ///   2. Busca a Invoice vinculada.
    ///   3. Chama MarkAsPaid() na Invoice (regra de negócio no Domínio).
    ///   4. Atualiza o status do PixCharge para Paid.
    ///   5. Registra AuditLog imutável.
    ///   6. Persiste tudo em transação atômica (UnitOfWork).
    /// </summary>
    public class ProcessWebhookHandler : IRequestHandler<ProcessWebhookCommand, Unit>
    {
        private readonly IPixChargeRepository _pixChargeRepository;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ProcessWebhookHandler(
            IPixChargeRepository pixChargeRepository,
            IInvoiceRepository invoiceRepository,
            IAuditLogRepository auditLogRepository,
            IUnitOfWork unitOfWork)
        {
            _pixChargeRepository = pixChargeRepository;
            _invoiceRepository = invoiceRepository;
            _auditLogRepository = auditLogRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(ProcessWebhookCommand request, CancellationToken cancellationToken)
        {
            // 1. Localiza o PixCharge pelo TxId (chave do BACEN)
            var txId = new TxId(request.TxId);
            var pixCharge = await _pixChargeRepository.GetByTxIdAsync(txId);
            if (pixCharge == null)
                throw new DomainException($"PixCharge com TxId '{request.TxId}' não encontrado.");

            // 2. Localiza a Invoice vinculada ao PixCharge
            var invoice = await _invoiceRepository.GetByIdAsync(pixCharge.InvoiceId);
            if (invoice == null)
                throw new DomainException($"Invoice com Id '{pixCharge.InvoiceId}' não encontrada.");

            // 3. Aplica a regra de negócio no Domínio (transição Open -> Paid)
            invoice.MarkAsPaid();

            // 4. Atualiza o status da cobrança Pix para Paid
            pixCharge.UpdateStatus(PixChargeStatus.Paid);

            // 5. Persiste as alterações
            await _invoiceRepository.UpdateAsync(invoice);
            await _pixChargeRepository.UpdateAsync(pixCharge);

            // 6. Registra o log de auditoria imutável com o payload bruto para rastreabilidade
            var auditLog = new AuditLog(
                entityId: invoice.Id,
                action: AuditAction.PaymentReceived,
                changes: $"Pagamento confirmado via webhook. TxId: {request.TxId}. " +
                         $"Valor pago: {request.PaidAmount:C}. Payload: {request.RawPayload}"
            );
            await _auditLogRepository.AddAsync(auditLog);

            // 7. Confirma transação atômica — nada é salvo pela metade
            await _unitOfWork.CommitAsync();

            return Unit.Value;
        }
    }
}
