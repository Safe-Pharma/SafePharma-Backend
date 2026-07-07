using FluentValidation;

namespace SafePharma.BLL
{
    public class PaymentMethodUpsertDtoValidator : AbstractValidator<PaymentMethodUpsertDto>
    {
        public PaymentMethodUpsertDtoValidator()
        {
            RuleFor(x => x.MethodName).NotEmpty();
            RuleFor(x => x.Fields).NotEmpty().WithMessage("At least one field (e.g. handle, IBAN) is required.");
        }
    }
}
