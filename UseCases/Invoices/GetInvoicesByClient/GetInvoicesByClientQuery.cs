using System;
using System.Collections.Generic;
using HubBancario.Application.DTOs;
using HubBancario.Domain.Aggregates.Invoice;
using MediatR;

namespace HubBancario.Application.UseCases.Invoices.GetInvoicesByClient
{
    /// <summary>
    /// Query para listar as faturas de um cliente, com filtro opcional de status.
    /// Não altera estado — apenas leitura (CQRS).
    /// </summary>
    public class GetInvoicesByClientQuery : IRequest<IEnumerable<InvoiceDto>>
    {
        public Guid ClientId { get; init; }

        /// <summary>Filtro opcional por status (null = todos os status).</summary>
        public InvoiceStatus? Status { get; init; }

        public GetInvoicesByClientQuery(Guid clientId, InvoiceStatus? status = null)
        {
            ClientId = clientId;
            Status = status;
        }
    }
}
