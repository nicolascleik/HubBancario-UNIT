using System;
using HubBancario.Application.DTOs;
using MediatR;

namespace HubBancario.Application.Queries.Account
{
    public class GetAccountByIdQuery : IRequest<AccountDto>
    {
        public Guid Id { get; set; }

        public GetAccountByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}