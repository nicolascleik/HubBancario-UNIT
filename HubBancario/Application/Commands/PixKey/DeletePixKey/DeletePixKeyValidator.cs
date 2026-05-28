using FluentValidation;

namespace HubBancario.Application.Commands.PixKey.DeletePixKey
{
    public class DeletePixKeyValidator : AbstractValidator<DeletePixKeyCommand>
    {
        public DeletePixKeyValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("O Id da chave Pix é obrigatório para realizar a exclusão.");
        }
    }
}