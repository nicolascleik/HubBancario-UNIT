using System;
using MediatR;

namespace HubBancario.Application.Commands.CreateInvoice
{
    public class CreateInvoiceCommand : IRequest<Guid>
    {
        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public string ExternalReference { get; set; }
    }
}