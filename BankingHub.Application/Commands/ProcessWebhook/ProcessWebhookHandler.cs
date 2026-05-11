using System;
using System.Threading;
using System.Threading.Tasks;
using BankingHub.Application.Interfaces;
using HubBancario.Domain.Repositories;
using MediatR;

namespace BankingHub.Application.Commands.ProcessWebhook
{
    public class ProcessWebhookHandler : IRequestHandler<ProcessWebhookCommand>
    {
        private readonly IPixChargeRepository _pixChargeRepository;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly INotificationService _notificationService;
        private readonly IUnitOfWork _unitOfWork;

        public ProcessWebhookHandler(
            IPixChargeRepository pixChargeRepository,
            IInvoiceRepository invoiceRepository,
            IClientRepository clientRepository,
            IAuditLogRepository auditLogRepository,
            INotificationService notificationService,
            IUnitOfWork unitOfWork)
        {
            _pixChargeRepository = pixChargeRepository;
            _invoiceRepository = invoiceRepository;
            _clientRepository = clientRepository;
            _auditLogRepository = auditLogRepository;
            _notificationService = notificationService;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(ProcessWebhookCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
