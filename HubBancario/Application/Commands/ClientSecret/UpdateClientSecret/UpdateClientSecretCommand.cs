using System;
using MediatR;

namespace HubBancario.Application.Commands.ClientSecret.UpdateClientSecret
{
    public class UpdateClientSecretCommand : IRequest
    {
        public Guid Id { get; set; }
        public string Certificate { get; set; }
        public string CertificatePassword { get; set; }
    }
}