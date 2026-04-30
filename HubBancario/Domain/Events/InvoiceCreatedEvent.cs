using System;
using HubBancario.Domain.ValueObjects;

namespace HubBancario.Domain.Events
{
    public record InvoiceCreatedEvent(Guid InvoiceId, Money Amount, DateTime CreatedAt)
    {
        public InvoiceCreatedEvent() : this(Guid.Empty, null, DateTime.MinValue)
        {
        }
    }
}

