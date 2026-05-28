using System;
using MediatR;
using HubBancario.Domain.ValueObjects;

namespace HubBancario.Domain.Events
{
    /// <summary>
    /// Evento disparado quando uma nova fatura (Invoice) é criada com sucesso.
    /// </summary>
    public sealed record InvoiceCreatedEvent(
        Guid InvoiceId,
        Guid AccountId,
        Money Amount,
        DateTime DueDate,
        string ExternalReference
    ) : INotification;
}