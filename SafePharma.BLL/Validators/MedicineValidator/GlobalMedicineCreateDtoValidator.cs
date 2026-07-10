using FluentValidation;

namespace SafePharma.BLL
{
    public class GlobalMedicineCreateDtoValidator : AbstractValidator<GlobalMedicineCreateDto>
    {
        public GlobalMedicineCreateDtoValidator()
        {
            RuleFor(x => x.TradeNameAr).NotEmpty().MaximumLength(255);
            RuleFor(x => x.TradeNameEn).NotEmpty().MaximumLength(255);
            RuleFor(x => x.ScientificName).NotEmpty().MaximumLength(255);
            RuleFor(x => x.Category).NotEmpty().MaximumLength(50);
            RuleFor(x => x.UnitOfSale).NotEmpty().MaximumLength(50);
            RuleFor(x => x.UnitsPerPackage).GreaterThan(0);
            RuleFor(x => x.DosageForm).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Strength).NotEmpty().MaximumLength(50);
            RuleFor(x => x.TherapeuticCategory).MaximumLength(100);
            RuleFor(x => x.Manufacturer).MaximumLength(255);
            RuleFor(x => x.CountryOfOrigin).MaximumLength(100);
            RuleFor(x => x.StorageConditions).MaximumLength(100);
        }
    }
}