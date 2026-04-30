using System;
using HubBancario.Domain.ValueObjects;

namespace HubBancario.Domain.Aggregates.Invoice
{
    public class Invoice
    {
        public Guid Id { get; private set; }
        public Guid ClientId { get; private set; }
        public Money Amount { get; private set; }
        public DateTime DueDate { get; private set; }
        public InvoiceStatus Status { get; private set; }
        public string ExternalReference { get; private set; }

        public void MarkAsPaid()
        {
            throw new NotImplementedException();
        }

        public void Cancel()
        {
            throw new NotImplementedException();
        }
    }
}

