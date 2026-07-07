using FluentValidation;

namespace SafePharma.BLL
{
    public class RejectPaymentDtoValidator : AbstractValidator<RejectPaymentDto>
    {
        public RejectPaymentDtoValidator()
        {
            RuleFor(x => x.RejectionReason).NotEmpty().WithMessage("A rejection reason is required.");
        }
    }
}