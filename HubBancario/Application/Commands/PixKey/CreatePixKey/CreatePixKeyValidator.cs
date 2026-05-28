using System;
using FluentValidation;

namespace HubBancario.Application.Commands.PixKey.CreatePixKey
{
    public class CreatePixKeyValidator : AbstractValidator<CreatePixKeyCommand>
    {
        public CreatePixKeyValidator()
        {
            RuleFor(x => x.KeyValue)
                .NotEmpty().WithMessage("O valor da chave Pix é obrigatório.");

            RuleFor(x => x.AccountId)
                .NotEmpty().WithMessage("O AccountId é obrigatório para vincular a chave à conta.");
        }
    }
}