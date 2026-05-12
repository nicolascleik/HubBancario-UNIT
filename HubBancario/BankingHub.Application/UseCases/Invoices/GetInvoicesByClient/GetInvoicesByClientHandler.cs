using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HubBancario.Application.DTOs;
using HubBancario.Domain.Aggregates.Invoice;
using HubBancario.Domain.Exceptions;
using HubBancario.Domain.Repositories;
using MediatR;

namespace HubBancario.Application.UseCases.Invoices.GetInvoicesByClient
{
    /// <summary>
    /// Handler da query GetInvoicesByClientQuery.
    /// Lista as faturas do cliente, aplicando o filtro de status quando informado.
    /// </summary>
    public class GetInvoicesByClientHandler : IRequestHandler<GetInvoicesByClientQuery, IEnumerable<InvoiceDto>>
    {
        private readonly IClientRepository _clientRepository;
        private readonly IInvoiceRepository _invoiceRepository;

        public GetInvoicesByClientHandler(
            IClientRepository clientRepository,
            IInvoiceRepository invoiceRepository)
        {
            _clientRepository = clientRepository;
            _invoiceRepository = invoiceRepository;
        }

        public async Task<IEnumerable<InvoiceDto>> Handle(GetInvoicesByClientQuery request, CancellationToken cancellationToken)
        {
            // Valida se o cliente existe antes de consultar
            var client = await _clientRepository.GetByIdAsync(request.ClientId);
            if (client == null)
                throw new DomainException($"Cliente com Id '{request.ClientId}' não encontrado.");

            var invoices = await _invoiceRepository.GetByClientIdAsync(request.ClientId);

            // Aplica filtro de status se fornecido
            if (request.Status.HasValue)
                invoices = invoices.Where(i => i.Status == request.Status.Value);

            return invoices.Select(invoice => new InvoiceDto
            {
                Id = invoice.Id,
                ClientId = invoice.ClientId,
                Amount = invoice.Amount.Value,
                Currency = invoice.Amount.Currency,
                DueDate = invoice.DueDate,
                Status = invoice.Status.ToString(),
                ExternalReference = invoice.ExternalReference
            });
        }
    }
}
