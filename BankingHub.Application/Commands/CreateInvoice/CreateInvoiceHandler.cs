using System;
using System.Threading;
using System.Threading.Tasks;
using HubBancario.Domain.Aggregates.Audit;
using HubBancario.Domain.Aggregates.Invoice;
using HubBancario.Domain.Repositories;
using MediatR;

namespace BankingHub.Application.Commands.CreateInvoice
{
    public class CreateInvoiceHandler : IRequestHandler<CreateInvoiceCommand, Guid>
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateInvoiceHandler(
            IInvoiceRepository invoiceRepository,
            IAuditLogRepository auditLogRepository,
            IUnitOfWork unitOfWork)
        {
            _invoiceRepository = invoiceRepository;
            _auditLogRepository = auditLogRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
