using HubBancario.Domain.Exceptions;
using System;

namespace HubBancario.Domain.Aggregates.Account;

/// <summary>
/// Entidade que guarda os certificados digitais e credenciais do ERP do cliente.
/// </summary>
public class ClientSecret
{
    public Guid Id { get; private set; }
    public Guid AccountId { get; private set; }
    public string SecretValue { get; private set; }
    public string Certificate { get; private set; }
    public string CertificatePassword { get; private set; }
    public bool IsValid { get; private set; }

    protected ClientSecret() { }

    public static ClientSecret Create(Guid accountId, string secretValue, string certificate, string certificatePassword)
    {
        if (accountId == Guid.Empty) throw new DomainException("O AccountId é obrigatório.");
        if (string.IsNullOrWhiteSpace(secretValue)) throw new DomainException("O SecretValue é obrigatório.");
        if (string.IsNullOrWhiteSpace(certificate)) throw new DomainException("O Certificado é obrigatório.");
        if (string.IsNullOrWhiteSpace(certificatePassword)) throw new DomainException("A Senha do Certificado é obrigatória.");

        return new ClientSecret
        {
            Id = Guid.NewGuid(),
            AccountId = accountId,
            SecretValue = secretValue,
            Certificate = certificate,
            CertificatePassword = certificatePassword,
            IsValid = true
        };
    }

    public void UpdateCertificate(string newCertificate, string newPassword)
    {
        if (!IsValid) throw new DomainException("Não é possível atualizar um certificado em uma credencial revogada.");
        if (string.IsNullOrWhiteSpace(newCertificate)) throw new DomainException("O novo Certificado é obrigatório.");
        if (string.IsNullOrWhiteSpace(newPassword)) throw new DomainException("A nova Senha do Certificado é obrigatória.");

        Certificate = newCertificate;
        CertificatePassword = newPassword;
    }

    public void UpdateSecret(string newSecret)
    {
        if (!IsValid) throw new DomainException("Não é possível atualizar a senha em uma credencial revogada.");
        if (string.IsNullOrWhiteSpace(newSecret)) throw new DomainException("O novo SecretValue é obrigatório.");

        SecretValue = newSecret;
    }

    public void Revoke()
    {
        if (!IsValid) throw new DomainException("Esta credencial já está revogada.");
        IsValid = false;
    }
}