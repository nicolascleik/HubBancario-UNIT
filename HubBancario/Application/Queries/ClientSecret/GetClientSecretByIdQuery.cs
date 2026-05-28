using System;
using HubBancario.Application.DTOs;
using MediatR;

namespace HubBancario.Application.Queries.ClientSecret
{
    public class GetClientSecretByIdQuery : IRequest<ClientSecretDto>
    {
        public Guid Id { get; set; }

        public GetClientSecretByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}