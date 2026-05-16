using HubBancario.Domain.Exceptions;
using System;

namespace HubBancario.Domain.Aggregates.Account;

/// <summary>
/// Entidade que gerencia as chaves Pix vinculadas a uma Conta específica.
/// </summary>
public class PixKey
{
    public Guid Id { get; private set; }
    public string KeyValue { get; private set; }
    public Guid AccountId { get; private set; }
    public bool IsActive { get; private set; }

    protected PixKey() { }

    public static PixKey Create(string keyValue, Guid accountId)
    {
        if (string.IsNullOrWhiteSpace(keyValue)) throw new DomainException("O valor da chave Pix é obrigatório.");
        if (accountId == Guid.Empty) throw new DomainException("O AccountId é obrigatório.");

        return new PixKey
        {
            Id = Guid.NewGuid(),
            KeyValue = keyValue,
            AccountId = accountId,
            IsActive = true
        };
    }

    public void UpdateKey(string newKeyValue)
    {
        if (!IsActive) throw new DomainException("Não é possível alterar uma chave inativa.");
        if (string.IsNullOrWhiteSpace(newKeyValue)) throw new DomainException("O novo valor da chave Pix é obrigatório.");

        KeyValue = newKeyValue;
    }

    public void Delete()
    {
        if (!IsActive) throw new DomainException("Esta chave Pix já está inativa ou deletada.");
        IsActive = false;
    }
}