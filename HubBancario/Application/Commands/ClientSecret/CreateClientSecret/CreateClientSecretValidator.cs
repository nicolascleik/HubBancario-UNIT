using FluentValidation;

namespace HubBancario.Application.Commands.ClientSecret.CreateClientSecret
{
    public class CreateClientSecretValidator : AbstractValidator<CreateClientSecretCommand>
    {
        public CreateClientSecretValidator()
        {
            RuleFor(x => x.AccountId)
                .NotEmpty().WithMessage("O AccountId é obrigatório para vincular as credenciais à conta.");

            RuleFor(x => x.SecretValue)
                .NotEmpty().WithMessage("O SecretValue é obrigatório.");

            RuleFor(x => x.Certificate)
                .NotEmpty().WithMessage("O Certificado é obrigatório.");

            RuleFor(x => x.CertificatePassword)
                .NotEmpty().WithMessage("A Senha do Certificado é obrigatória.");
        }
    }
}