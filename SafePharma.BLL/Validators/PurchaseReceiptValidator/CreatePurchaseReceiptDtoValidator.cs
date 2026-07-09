using FluentValidation;

namespace SafePharma.BLL
{
    public class CreatePurchaseReceiptDtoValidator
    : AbstractValidator<CreatePurchaseReceiptDto>
    {
        public CreatePurchaseReceiptDtoValidator()
        {
            RuleFor(x => x.InvoiceNumber)
            .NotEmpty()
            .WithMessage("Invoice number is required.")
            .MaximumLength(100)
            .WithMessage("Invoice number cannot exceed 100 characters.");

        RuleFor(x => x.InvoiceDate)
            .NotEmpty()
            .WithMessage("Invoice date is required.");

            RuleFor(x => x.InvoiceTotal)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Invoice total must be greater than or equal to zero.");

            RuleFor(x => x.Items)
                .NotEmpty()
                .WithMessage("At least one receipt item is required.");

            RuleForEach(x => x.Items)
                .SetValidator(new CreatePurchaseReceiptItemDtoValidator());
        }
    }

}
