using FluentValidation;

namespace SafePharma.BLL
{
    public class PharmacyMedicineUpdateDtoValidator : AbstractValidator<PharmacyMedicineUpdateDto>
    {
        public PharmacyMedicineUpdateDtoValidator()
        {
            RuleFor(x => x.TaxIds).NotEmpty().WithMessage("At least one tax is required.");
            RuleForEach(x => x.TaxIds).NotEmpty();
            RuleFor(x => x.PurchasePrice).GreaterThanOrEqualTo(0);
            RuleFor(x => x.SellingPrice).GreaterThanOrEqualTo(0);
            RuleFor(x => x.MinStockLevel).GreaterThanOrEqualTo(0);
        }
    }
}