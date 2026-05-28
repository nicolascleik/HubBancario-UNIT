using System.Linq;
using FluentValidation;

namespace HubBancario.Application.Commands.Account.CreateAccount
{
    public class CreateAccountValidator : AbstractValidator<CreateAccountCommand>
    {
        public CreateAccountValidator()
        {
            RuleFor(x => x.SecretId)
                .NotEmpty().WithMessage("O SecretId (credencial) é obrigatório.");

            RuleFor(x => x.Document)
                .NotEmpty().WithMessage("O Documento (CPF/CNPJ) é obrigatório.")
                .Must(BeAValidDocumentLength).WithMessage("O documento deve ter 11 (CPF) ou 14 (CNPJ) dígitos numéricos.");

            RuleFor(x => x.BankId)
                .NotEmpty().WithMessage("O código do banco (BankId) é obrigatório.");

            RuleFor(x => x.AccountNumber)
                .NotEmpty().WithMessage("O número da conta é obrigatório.");

            RuleFor(x => x.Agency)
                .NotEmpty().WithMessage("A agência é obrigatória.");
        }

        // Validação inicial simples (O Domínio fará a validação matemática real)
        private bool BeAValidDocumentLength(string document)
        {
            if (string.IsNullOrWhiteSpace(document)) return false;
            
            // Remove as máscaras para contar apenas os números
            var numericOnly = new string(document.Where(char.IsDigit).ToArray());
            return numericOnly.Length == 11 || numericOnly.Length == 14;
        }
    }
}