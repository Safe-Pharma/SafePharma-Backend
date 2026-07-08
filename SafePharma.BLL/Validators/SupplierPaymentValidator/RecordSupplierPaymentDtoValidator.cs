using FluentValidation;
using SafePharma.DAL;

namespace SafePharma.BLL
{
    public class RecordSupplierPaymentDtoValidator : AbstractValidator<RecordSupplierPaymentDto>
    {
        public RecordSupplierPaymentDtoValidator()
        {
            RuleFor(x => x.SupplierId)
                .NotEmpty().WithMessage("Supplier is required.");

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Amount must be greater than 0.");

            RuleFor(x => x.PaymentMethod)
                .NotEmpty().WithMessage("Payment method is required.")
                .Must(m => SupplierPaymentMethods.All.Contains(m))
                .WithMessage($"Payment method must be one of: {string.Join(", ", SupplierPaymentMethods.All)}.");

            RuleFor(x => x.PaidAt)
                .NotEmpty().WithMessage("Payment date is required.")
                .LessThanOrEqualTo(DateTime.UtcNow.AddDays(1))
                .WithMessage("Payment date cannot be in the future.");

            RuleFor(x => x.Note)
                .MaximumLength(1000);
        }
    }
}
