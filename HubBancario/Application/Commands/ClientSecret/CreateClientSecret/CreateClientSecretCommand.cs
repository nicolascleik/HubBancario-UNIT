using System;
using MediatR;

namespace HubBancario.Application.Commands.ClientSecret.CreateClientSecret
{
    public class CreateClientSecretCommand : IRequest<Guid>
    {
        public Guid AccountId { get; set; }
        public string SecretValue { get; set; }
        public string Certificate { get; set; }
        public string CertificatePassword { get; set; }
    }
}