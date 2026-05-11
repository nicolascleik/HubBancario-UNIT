using System;
using MediatR;

namespace BankingHub.Application.Commands.CreateInvoice
{
    public class CreateInvoiceCommand : IRequest<Guid>
    {
        public Guid ClientId { get; set; }
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public string ExternalReference { get; set; }
    }
}
