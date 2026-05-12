using FluentValidation;

namespace BankingHub.Application.Commands.CreateInvoice
{
    public class CreateInvoiceValidator : AbstractValidator<CreateInvoiceCommand>
    {
        public CreateInvoiceValidator()
        {
            RuleFor(x => x.ClientId)
                .NotEmpty().WithMessage("ClientId é obrigatório.");

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("O valor deve ser maior que zero.");

            RuleFor(x => x.DueDate)
                .GreaterThan(DateTime.UtcNow).WithMessage("A data de vencimento deve ser futura.");
        }
    }
}
