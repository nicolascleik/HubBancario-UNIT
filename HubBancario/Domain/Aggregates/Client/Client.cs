using System;
using HubBancario.Domain.ValueObjects;

namespace HubBancario.Domain.Aggregates.Client
{
    public class Client
    {
        public Guid Id { get; private set; }
        public string CompanyName { get; private set; }
        public Document Document { get; private set; }
        public bool IsActive { get; private set; }
        public string DefaultBankId { get; private set; }
        public string WebhookUrl { get; private set; }

        public void Deactivate()
        {
            throw new NotImplementedException();
        }

        public void UpdateWebhook(string url)
        {
            throw new NotImplementedException();
        }
    }
}

