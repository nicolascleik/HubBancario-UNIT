using FluentValidation;

namespace HubBancario.Application.Commands.Account.DeleteAccount
{
    public class DeleteAccountValidator : AbstractValidator<DeleteAccountCommand>
    {
        public DeleteAccountValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("O Id da conta é obrigatório para realizar a inativação.");
        }
    }
}