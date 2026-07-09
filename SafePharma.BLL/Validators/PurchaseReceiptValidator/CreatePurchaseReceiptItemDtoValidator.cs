using FluentValidation;

namespace SafePharma.BLL
{
    public class CreatePurchaseReceiptItemDtoValidator
    : AbstractValidator<CreatePurchaseReceiptItemDto>
    {
        public CreatePurchaseReceiptItemDtoValidator()
        {
            RuleFor(x => x.PurchaseOrderItemId)
            .NotEmpty()
            .WithMessage("Purchase order item id is required.");

        RuleFor(x => x.BatchNumber)
            .NotEmpty()
            .WithMessage("Batch number is required.")
            .MaximumLength(100)
            .WithMessage("Batch number cannot exceed 100 characters.");

            RuleFor(x => x.ExpiryDate)
                .GreaterThan(DateTime.UtcNow)
                .WithMessage("Expiry date must be in the future.");
        }
    }

}

