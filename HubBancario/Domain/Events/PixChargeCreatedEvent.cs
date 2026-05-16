using System;
using MediatR;
using HubBancario.Domain.ValueObjects;
using HubBancario.Domain.Aggregates.PixCharge;

namespace HubBancario.Domain.Events
{
    /// <summary>
    /// Evento disparado assim que o QR Code Pix é oficialmente gerado e registado.
    /// </summary>
    public sealed record PixChargeCreatedEvent(
        TxId TxId,
        Guid InvoiceId,
        PixChargeType ChargeType,
        string Emv
    ) : INotification;
}