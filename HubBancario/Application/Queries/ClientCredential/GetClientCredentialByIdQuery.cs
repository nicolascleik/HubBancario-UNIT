using System;
using HubBancario.Application.DTOs;
using MediatR;

namespace HubBancario.Application.Queries.ClientCredential
{
    /// <summary>
    /// Query que solicita a busca de uma credencial de cliente pelo seu identificador único.
    /// Segue o padrão CQRS: apenas leitura, sem efeitos colaterais.
    /// </summary>
    public class GetClientCredentialByIdQuery : IRequest<ClientCredentialDto>
    {
        public Guid Id { get; set; }

        public GetClientCredentialByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
