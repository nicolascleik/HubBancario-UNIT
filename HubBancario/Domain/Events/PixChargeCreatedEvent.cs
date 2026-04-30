using System;
using HubBancario.Domain.ValueObjects;

namespace HubBancario.Domain.Events
{
    public record PixChargeCreatedEvent(TxId TxId, Guid InvoiceId, DateTime CreatedAt)
    {
        public PixChargeCreatedEvent() : this(null, Guid.Empty, DateTime.MinValue)
        {
        }
    }
}

