using System;
using HubBancario.Domain.ValueObjects;

namespace HubBancario.Domain.Events
{
    public record PaymentConfirmedEvent(Guid InvoiceId, TxId TxId, Money Amount, DateTime PaidAt)
    {
        public PaymentConfirmedEvent() : this(Guid.Empty, null, null, DateTime.MinValue)
        {
        }
    }
}

