using FluentValidation;

namespace HubBancario.Application.Commands.ClientSecret.UpdateClientSecret
{
    public class UpdateClientSecretValidator : AbstractValidator<UpdateClientSecretCommand>
    {
        public UpdateClientSecretValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("O Id da credencial é obrigatório para atualização.");

            RuleFor(x => x.Certificate)
                .NotEmpty().WithMessage("O novo Certificado é obrigatório.");

            RuleFor(x => x.CertificatePassword)
                .NotEmpty().WithMessage("A nova Senha do Certificado é obrigatória.");
        }
    }
}