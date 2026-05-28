using System;
using FluentValidation;

namespace HubBancario.Application.Commands.Account.UpdateAccount
{
    public class UpdateAccountValidator : AbstractValidator<UpdateAccountCommand>
    {
        public UpdateAccountValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("O Id da conta é obrigatório para atualização.");

            RuleFor(x => x.BankId)
                .NotEmpty().WithMessage("O código do banco (BankId) é obrigatório.");

            RuleFor(x => x.AccountNumber)
                .NotEmpty().WithMessage("O número da conta é obrigatório.");

            RuleFor(x => x.Agency)
                .NotEmpty().WithMessage("A agência é obrigatória.");
        }
    }
}