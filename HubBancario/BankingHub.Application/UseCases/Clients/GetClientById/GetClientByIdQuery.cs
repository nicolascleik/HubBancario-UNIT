using System;
using HubBancario.Application.DTOs;
using MediatR;

namespace HubBancario.Application.UseCases.Clients.GetClientById
{
    /// <summary>
    /// Query para buscar um cliente B2B pelo seu Id.
    /// Não altera estado — apenas leitura (CQRS).
    /// </summary>
    public class GetClientByIdQuery : IRequest<ClientDto>
    {
        public Guid ClientId { get; init; }

        public GetClientByIdQuery(Guid clientId)
        {
            ClientId = clientId;
        }
    }
}
