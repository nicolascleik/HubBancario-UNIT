using System;
using HubBancario.Application.DTOs;
using MediatR;

namespace HubBancario.Application.UseCases.Invoices.GetInvoiceById
{
    /// <summary>
    /// Query para buscar uma fatura específica pelo seu Id.
    /// Não altera estado — apenas leitura (CQRS).
    /// </summary>
    public class GetInvoiceByIdQuery : IRequest<InvoiceDto>
    {
        public Guid InvoiceId { get; init; }

        public GetInvoiceByIdQuery(Guid invoiceId)
        {
            InvoiceId = invoiceId;
        }
    }
}
