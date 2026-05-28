using System;
using MediatR;

namespace HubBancario.Application.Commands.Account.UpdateAccount
{
    // Implementa apenas IRequest (sem tipo de retorno) pois a API devolverá um 204 No Content
    public class UpdateAccountCommand : IRequest
    {
        public Guid Id { get; set; }
        public string BankId { get; set; }
        public string AccountNumber { get; set; }
        public string Agency { get; set; }
    }
}