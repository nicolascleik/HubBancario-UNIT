using FluentValidation;

namespace HubBancario.Application.Commands.CreatePixCharge
{
    public class CreatePixChargeValidator : AbstractValidator<CreatePixChargeCommand>
    {
        public CreatePixChargeValidator()
        {
            RuleFor(x => x.InvoiceId)
                .NotEmpty().WithMessage("InvoiceId é obrigatório.");
        }
    }
}
