using System.Threading;
using System.Threading.Tasks;
using HubBancario.Application.Interfaces;
using HubBancario.Domain.Aggregates.Audit;
using HubBancario.Domain.Aggregates.PixCharge;
using HubBancario.Domain.Exceptions;
using HubBancario.Domain.Repositories;
using HubBancario.Domain.ValueObjects;
using MediatR;

namespace HubBancario.Application.Commands.ProcessWebhook
{
    public class ProcessWebhookHandler : IRequestHandler<ProcessWebhookCommand>
    {
        private readonly IPixChargeRepository _pixChargeRepository;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly INotificationService _notificationService;
        private readonly IUnitOfWork _unitOfWork;

        public ProcessWebhookHandler(
            IPixChargeRepository pixChargeRepository,
            IInvoiceRepository invoiceRepository,
            IAuditLogRepository auditLogRepository,
            INotificationService notificationService,
            IUnitOfWork unitOfWork)
        {
            _pixChargeRepository = pixChargeRepository;
            _invoiceRepository = invoiceRepository;
            _auditLogRepository = auditLogRepository;
            _notificationService = notificationService;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(ProcessWebhookCommand request, CancellationToken cancellationToken)
        {
            // 1. Encontra qual cobrança foi paga
            var txId = TxId.From(request.TxId);
            var charge = await _pixChargeRepository.GetByTxIdAsync(txId);
            if (charge == null) throw new DomainException($"Nenhuma cobrança encontrada para o TxId {request.TxId}");

            var invoice = await _invoiceRepository.GetByIdAsync(charge.InvoiceId);

            // 2. Atualiza os status no nosso domínio
            charge.UpdateStatus(PixChargeStatus.Paid);
            invoice.MarkAsPaid();

            // 3. Grava o registro de auditoria intocável para o compliance bancário
            var auditLog = AuditLog.Register(
                accountId: invoice.AccountId,
                paymentStatus: "PAID",
                txId: txId,
                amount: invoice.Amount,
                payloadDetails: request.RawPayload
            );

            await _auditLogRepository.AddAsync(auditLog);

            // 4. Salva a transação atômica
            await _unitOfWork.CommitAsync();

            // 5. Envia o aviso HTTP (B2B) assíncrono para o ERP do lojista
            await _notificationService.NotifyClientAsync(invoice.AccountId, new { TxId = request.TxId, Status = "PAID" });
        }
    }
}