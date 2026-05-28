using System;
using MediatR;

namespace HubBancario.Application.Commands.ClientSecret.RevokeClientSecret
{
    // Retorna vazio (204 No Content) na API após a revogação
    public class RevokeClientSecretCommand : IRequest
    {
        public Guid Id { get; set; }
    }
}