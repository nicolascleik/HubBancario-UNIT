using System;
using MediatR;
using HubBancario.Application.DTOs;

namespace HubBancario.Application.Queries.GetInvoice
{
    public class GetInvoiceQuery : IRequest<InvoiceDto>
    {
        public Guid InvoiceId { get; set; }
    }
}