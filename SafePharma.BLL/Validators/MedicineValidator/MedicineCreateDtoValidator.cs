using FluentValidation;

namespace SafePharma.BLL
{
    public class MedicineCreateDtoValidator : AbstractValidator<MedicineCreateDto>
    {
        public MedicineCreateDtoValidator()
        {
            RuleFor(x => x.TradeNameAr).NotEmpty().MaximumLength(255);
            RuleFor(x => x.TradeNameEn).NotEmpty().MaximumLength(255);
            RuleFor(x => x.ScientificName).NotEmpty().MaximumLength(255);
            RuleFor(x => x.Category).NotEmpty().MaximumLength(50);
            RuleFor(x => x.UnitOfSale).NotEmpty().MaximumLength(50);
            RuleFor(x => x.UnitsPerPackage).GreaterThan(0);
            RuleFor(x => x.PurchasePrice).GreaterThanOrEqualTo(0);
            RuleFor(x => x.SellingPrice).GreaterThanOrEqualTo(0);
            RuleFor(x => x.TaxId).NotEmpty();
            RuleFor(x => x.MinStockLevel).GreaterThanOrEqualTo(0);
            RuleFor(x => x.TherapeuticCategory).MaximumLength(100);
            RuleFor(x => x.Manufacturer).MaximumLength(255);
            RuleFor(x => x.CountryOfOrigin).MaximumLength(100);
            RuleFor(x => x.StorageConditions).MaximumLength(100);
        }
    }
}