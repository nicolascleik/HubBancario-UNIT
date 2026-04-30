using System;

namespace HubBancario.Domain.Aggregates.Client;

public class ClientSecret
{
    public Guid Id { get; private set; }
    public Guid ClientId { get; private set; }
    public string SecretHash { get; private set; }
    public DateTime ExpiresAt { get; private set; }

    public bool IsValid()
    {
        throw new NotImplementedException();
    }
}