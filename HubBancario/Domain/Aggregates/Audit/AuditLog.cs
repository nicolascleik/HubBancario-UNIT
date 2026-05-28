using System;
using HubBancario.Domain.Exceptions;
using HubBancario.Domain.ValueObjects;

namespace HubBancario.Domain.Aggregates.Audit
{
    /// <summary>
    /// Aggregate Root imutável que representa o rastro detalhado e histórico das transações financeiras.
    /// </summary>
    public class AuditLog
    {
        public Guid Id { get; private set; }
        public Guid AccountId { get; private set; }
        public DateTime OccurredAt { get; private set; }
        public string PaymentStatus { get; private set; }
        public TxId TxId { get; private set; }
        public Money Amount { get; private set; }
        public string PayloadDetails { get; private set; }

        protected AuditLog() { }

        /// <summary>
        /// Método de fábrica (Factory Method) para registrar uma nova ocorrência imutável de auditoria.
        /// </summary>
        public static AuditLog Register(Guid accountId, string paymentStatus, TxId txId, Money amount, string payloadDetails)
        {
            if (accountId == Guid.Empty)
                throw new DomainException("O AccountId vinculativo da auditoria é obrigatório.");

            if (string.IsNullOrWhiteSpace(paymentStatus))
                throw new DomainException("O status do pagamento na auditoria não pode ser vazio.");

            if (txId == null)
                throw new DomainException("O TxId associado à transação auditada é obrigatório.");

            if (amount == null)
                throw new DomainException("O valor (Money) movimentado na transação é obrigatório.");

            if (string.IsNullOrWhiteSpace(payloadDetails))
                throw new DomainException("O payload detalhado com o JSON da requisição/banco é obrigatório para compliance.");

            return new AuditLog
            {
                Id = Guid.NewGuid(),
                AccountId = accountId,
                OccurredAt = DateTime.UtcNow,
                PaymentStatus = paymentStatus.ToUpper().Trim(),
                TxId = txId,
                Amount = amount,
                PayloadDetails = payloadDetails
            };
        }
    }
}