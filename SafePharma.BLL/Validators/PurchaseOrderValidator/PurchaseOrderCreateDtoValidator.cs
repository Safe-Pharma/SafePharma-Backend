using FluentValidation;

namespace SafePharma.BLL
{
    public class PurchaseOrderCreateDtoValidator : AbstractValidator<PurchaseOrderCreateDto>
    {
        public PurchaseOrderCreateDtoValidator()
        {
            RuleFor(x => x.SupplierId)
                .NotEmpty()
                .WithMessage("Supplier is required.");

            RuleFor(x => x.OrderDate)
                .NotEmpty()
                .WithMessage("Order date is required.");

            RuleFor(x => x.Items)
                .NotEmpty()
                .WithMessage("Order must have at least one item.");

            RuleForEach(x => x.Items).ChildRules(item =>
            {
                item.RuleFor(i => i.PharmacyMedicineId)
                    .NotEmpty()
                    .WithMessage("Medicine is required.");

                item.RuleFor(i => i.QuantityOrdered)
                    .GreaterThan(0)
                    .WithMessage("Quantity must be greater than 0.");

                item.RuleFor(i => i.UnitPrice)
                    .GreaterThan(0)
                    .WithMessage("Unit price must be greater than 0.");
            });
        }
    }
}
