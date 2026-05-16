using System;
using MediatR;
using HubBancario.Domain.ValueObjects;

namespace HubBancario.Domain.Events
{
    /// <summary>
    /// Evento disparado quando um pagamento é confirmado definitivamente via consulta ativa (Polling) ou Webhook validado.
    /// </summary>
    public sealed record PaymentConfirmedEvent(
        Guid InvoiceId,
        TxId TxId,
        Money Amount,
        DateTime PaidAt
    ) : INotification;
}