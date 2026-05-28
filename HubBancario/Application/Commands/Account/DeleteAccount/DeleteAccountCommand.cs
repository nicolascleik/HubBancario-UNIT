using System;
using MediatR;

namespace HubBancario.Application.Commands.Account.DeleteAccount
{
    // Implementa IRequest sem retorno, pois o Controller devolverá apenas um 204 No Content
    public class DeleteAccountCommand : IRequest
    {
        public Guid Id { get; set; }
    }
}