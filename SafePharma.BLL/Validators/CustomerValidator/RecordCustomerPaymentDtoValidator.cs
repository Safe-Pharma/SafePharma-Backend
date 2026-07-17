using FluentValidation;

namespace SafePharma.BLL
{
    public class RecordCustomerPaymentDtoValidator : AbstractValidator<RecordCustomerPaymentDto>
    {
        public RecordCustomerPaymentDtoValidator()
        {
            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Payment amount must be greater than zero.");
        }
    }
}