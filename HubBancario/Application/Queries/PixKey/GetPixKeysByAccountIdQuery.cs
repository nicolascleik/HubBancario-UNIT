using System;
using System.Collections.Generic;
using HubBancario.Application.DTOs;
using MediatR;

namespace HubBancario.Application.Queries.PixKey
{
    public class GetPixKeysByAccountIdQuery : IRequest<IEnumerable<PixKeyDto>>
    {
        public Guid AccountId { get; set; }

        public GetPixKeysByAccountIdQuery(Guid accountId)
        {
            AccountId = accountId;
        }
    }
}