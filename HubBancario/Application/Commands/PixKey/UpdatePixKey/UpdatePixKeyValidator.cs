using System;
using FluentValidation;

namespace HubBancario.Application.Commands.PixKey.UpdatePixKey
{
    public class UpdatePixKeyValidator : AbstractValidator<UpdatePixKeyCommand>
    {
        public UpdatePixKeyValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("O Id da chave Pix é obrigatório para realizar a atualização.");

            RuleFor(x => x.NewKeyValue)
                .NotEmpty().WithMessage("O novo valor da chave Pix é obrigatório.");
        }
    }
}