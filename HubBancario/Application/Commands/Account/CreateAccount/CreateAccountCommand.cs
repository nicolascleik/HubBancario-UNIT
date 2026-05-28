using System;
using MediatR;

namespace HubBancario.Application.Commands.Account.CreateAccount
{
    public class CreateAccountCommand : IRequest<Guid>
    {
        public Guid SecretId { get; set; }
        public string Document { get; set; }
        public string BankId { get; set; }
        public string AccountNumber { get; set; }
        public string Agency { get; set; }
    }
}