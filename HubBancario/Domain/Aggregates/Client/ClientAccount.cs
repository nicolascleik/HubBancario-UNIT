using System;
using HubBancario.Domain.ValueObjects;

namespace HubBancario.Domain.Aggregates.Client
{
    public class ClientAccount
    {
        public Guid Id { get; private set; }
        public Guid ClientId { get; private set; }
        public Money Balance { get; private set; }

        public void Credit(Money amount)
        {
            throw new NotImplementedException();
        }

        public void Debit(Money amount)
        {
            throw new NotImplementedException();
        }
    }
}

