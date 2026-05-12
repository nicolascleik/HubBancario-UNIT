using System;
using MediatR;

namespace BankingHub.Application.Queries.GetInvoice
{
    public class GetInvoiceQuery : IRequest<InvoiceDto>
    {
        public Guid InvoiceId { get; set; }
    }
}
