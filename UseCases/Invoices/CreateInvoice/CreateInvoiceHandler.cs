using System;
using System.Threading;
using System.Threading.Tasks;
using HubBancario.Application.DTOs;
using HubBancario.Domain.Aggregates.Audit;
using HubBancario.Domain.Aggregates.Invoice;
using HubBancario.Domain.Exceptions;
using HubBancario.Domain.Repositories;
using HubBancario.Domain.ValueObjects;
using MediatR;

namespace HubBancario.Application.UseCases.Invoices.CreateInvoice
{
    /// <summary>
    /// Handler do comando CreateInvoiceCommand.
    /// Orquestra: busca o cliente, cria a Invoice no domínio,
    /// persiste tudo em transação atômica e registra no AuditLog.
    /// </summary>
    public class CreateInvoiceHandler : IRequestHandler<CreateInvoiceCommand, InvoiceDto>
    {
        private readonly IClientRepository _clientRepository;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateInvoiceHandler(
            IClientRepository clientRepository,
            IInvoiceRepository invoiceRepository,
            IAuditLogRepository auditLogRepository,
            IUnitOfWork unitOfWork)
        {
            _clientRepository = clientRepository;
            _invoiceRepository = invoiceRepository;
            _auditLogRepository = auditLogRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<InvoiceDto> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
        {
            // 1. Valida existência do cliente
            var client = await _clientRepository.GetByIdAsync(request.ClientId);
            if (client == null)
                throw new DomainException($"Cliente com Id '{request.ClientId}' não encontrado.");

            // 2. Cria a entidade Invoice (regra de negócio fica no Domínio)
            var invoice = new Invoice(
                clientId: client.Id,
                amount: new Money(request.Amount),
                dueDate: request.DueDate,
                externalReference: request.ExternalReference
            );

            // 3. Persiste a Invoice
            await _invoiceRepository.AddAsync(invoice);

            // 4. Registra log imutável de auditoria
            var auditLog = new AuditLog(
                entityId: invoice.Id,
                action: AuditAction.EntityCreated,
                changes: $"Invoice criada para o cliente '{client.CompanyName}' no valor de {invoice.Amount.Value:C}."
            );
            await _auditLogRepository.AddAsync(auditLog);

            // 5. Confirma transação atômica (UnitOfWork)
            await _unitOfWork.CommitAsync();

            return MapToDto(invoice);
        }

        private static InvoiceDto MapToDto(Invoice invoice) => new InvoiceDto
        {
            Id = invoice.Id,
            ClientId = invoice.ClientId,
            Amount = invoice.Amount.Value,
            Currency = invoice.Amount.Currency,
            DueDate = invoice.DueDate,
            Status = invoice.Status.ToString(),
            ExternalReference = invoice.ExternalReference
        };
    }
}
