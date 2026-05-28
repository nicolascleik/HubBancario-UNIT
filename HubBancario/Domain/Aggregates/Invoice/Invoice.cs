using System;
using HubBancario.Domain.Exceptions;
using HubBancario.Domain.ValueObjects;

namespace HubBancario.Domain.Aggregates.Invoice
{
    public class Invoice
    {
        public Guid Id { get; private set; }

        public Guid AccountId { get; private set; }

        public Money Amount { get; private set; }
        public DateTime DueDate { get; private set; }
        public InvoiceStatus Status { get; private set; }
        public string ExternalReference { get; private set; }

        protected Invoice() { }

        public static Invoice Create(Guid accountId, Money amount, DateTime dueDate, string externalReference)
        {
            if (accountId == Guid.Empty) throw new DomainException("O AccountId é obrigatório.");
            if (amount == null || amount.Value <= 0) throw new DomainException("O valor da fatura deve ser maior que zero.");
            if (string.IsNullOrWhiteSpace(externalReference)) throw new DomainException("A referência externa é obrigatória.");

            return new Invoice
            {
                Id = Guid.NewGuid(),
                AccountId = accountId,
                Amount = amount,
                DueDate = dueDate,
                Status = InvoiceStatus.Open,
                ExternalReference = externalReference
            };
        }

        public void MarkAsPaid()
        {
            // Idempotência: Se já estiver pago, não faz nada
            if (Status == InvoiceStatus.Paid) return;

            if (Status == InvoiceStatus.Canceled)
                throw new DomainException("Não é possível pagar uma fatura cancelada.");

            Status = InvoiceStatus.Paid;
        }

        public void Cancel()
        {
            if (Status == InvoiceStatus.Paid)
                throw new DomainException("Não é possível cancelar uma fatura que já foi paga.");

            Status = InvoiceStatus.Canceled;
        }
    }
}