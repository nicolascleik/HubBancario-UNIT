using HubBancario.Domain.Exceptions;
using HubBancario.Domain.ValueObjects;
using System;

namespace HubBancario.Domain.Aggregates.Account;

/// <summary>
/// Aggregate Root que representa a CONTA financeira e os dados bancários do lojista.
/// </summary>
public class Account
{
    public Guid Id { get; private set; }
    public Guid SecretId { get; private set; }
    public Document Document { get; private set; }
    public string BankId { get; private set; }
    public string AccountNumber { get; private set; }
    public string Agency { get; private set; }
    public bool IsActive { get; private set; }

    protected Account() { }

    public static Account Create(Guid secretId, Document document, string bankId, string accountNumber, string agency)
    {
        if (secretId == Guid.Empty) throw new DomainException("O SecretId é obrigatório.");
        if (document == null) throw new DomainException("O Documento é obrigatório.");
        if (string.IsNullOrWhiteSpace(bankId)) throw new DomainException("O BankId é obrigatório.");
        if (string.IsNullOrWhiteSpace(accountNumber)) throw new DomainException("O número da conta é obrigatório.");
        if (string.IsNullOrWhiteSpace(agency)) throw new DomainException("A agência é obrigatória.");

        return new Account
        {
            Id = Guid.NewGuid(),
            SecretId = secretId,
            Document = document,
            BankId = bankId,
            AccountNumber = accountNumber,
            Agency = agency,
            IsActive = true
        };
    }

    public void UpdateDetails(string bankId, string accountNumber, string agency)
    {
        if (!IsActive) throw new DomainException("Não é possível alterar dados de uma conta inativa.");
        if (string.IsNullOrWhiteSpace(bankId)) throw new DomainException("O BankId é obrigatório.");
        if (string.IsNullOrWhiteSpace(accountNumber)) throw new DomainException("O número da conta é obrigatório.");
        if (string.IsNullOrWhiteSpace(agency)) throw new DomainException("A agência é obrigatória.");

        BankId = bankId;
        AccountNumber = accountNumber;
        Agency = agency;
    }

    public void Deactivate()
    {
        if (!IsActive) throw new DomainException("Esta conta já está inativa.");
        IsActive = false;
    }
}