using FluentValidation;
using SafePharma.BLL.SafePharma.BLL;

namespace SafePharma.BLL
{
    public class SubmitPaymentProofDtoValidator : AbstractValidator<SubmitPaymentProofDto>
    {
        public SubmitPaymentProofDtoValidator()
        {
            RuleFor(x => x.PaymentMethod).NotEmpty();
            RuleFor(x => x.TransactionReference).NotEmpty();
            RuleFor(x => x.PaymentDate).NotEmpty().LessThanOrEqualTo(DateTime.UtcNow);
            RuleFor(x => x.PaidAmount).GreaterThan(0);
            RuleFor(x => x.ReceiptUrl)
                .NotEmpty().WithMessage("A receipt must be uploaded first via /proof/receipt.");
        }
    }
}