using System;
using HubBancario.Domain.ValueObjects;

namespace HubBancario.Domain.Aggregates.PixCharge
{
    public class PixCharge
    {
        public TxId TxId { get; private set; }
        public Guid InvoiceId { get; private set; }
        public PixChargeType ChargeType { get; private set; }
        public PixChargeStatus Status { get; private set; }
        public string Emv { get; private set; }
        public string RawPayload { get; private set; }

        public void UpdateStatus(PixChargeStatus newStatus)
        {
            throw new NotImplementedException();
        }
    }
}

