using System;
using MediatR;

namespace HubBancario.Application.Commands.Account.ChangeAccountStatus
{
    public class ChangeAccountStatusCommand : IRequest
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; }
    }
}