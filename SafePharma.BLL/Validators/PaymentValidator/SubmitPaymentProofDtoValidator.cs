using FluentValidation;

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
            RuleFor(x => x.Receipt).NotNull().WithMessage("A receipt image or screenshot is required.");
            RuleFor(x => x.Receipt)
                .NotNull().WithMessage("A receipt image or screenshot is required.")
                .Must(f => f.Length <= 5 * 1024 * 1024).WithMessage("Receipt must be under 5MB.")
                .Must(f => new[] { "image/jpeg", "image/png", "application/pdf" }.Contains(f.ContentType))
                .WithMessage("Receipt must be a JPG, PNG, or PDF.");
        }
    }
}