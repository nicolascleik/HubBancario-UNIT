using FluentValidation;

namespace HubBancario.Application.Commands.ClientSecret.RevokeClientSecret
{
    public class RevokeClientSecretValidator : AbstractValidator<RevokeClientSecretCommand>
    {
        public RevokeClientSecretValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("O Id da credencial é obrigatório para realizar a revogação.");
        }
    }
}