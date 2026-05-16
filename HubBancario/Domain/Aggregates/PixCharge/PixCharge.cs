using System;
using HubBancario.Domain.Exceptions;
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

        protected PixCharge() { }

        public static PixCharge Create(TxId txId, Guid invoiceId, PixChargeType chargeType, string emv, string rawPayload)
        {
            if (txId == null) throw new DomainException("O TxId é obrigatório.");
            if (invoiceId == Guid.Empty) throw new DomainException("O InvoiceId é obrigatório.");
            if (string.IsNullOrWhiteSpace(emv)) throw new DomainException("O código EMV (Copia e Cola) é obrigatório.");

            return new PixCharge
            {
                TxId = txId,
                InvoiceId = invoiceId,
                ChargeType = chargeType,
                Status = PixChargeStatus.Active, 
                Emv = emv,
                RawPayload = rawPayload
            };
        }

        public void UpdateStatus(PixChargeStatus newStatus)
        {
            if (Status == PixChargeStatus.Expired && newStatus == PixChargeStatus.Active)
            {
                throw new DomainException("Uma cobrança expirada não pode voltar a ficar ativa.");
            }

            Status = newStatus;
        }
    }
}