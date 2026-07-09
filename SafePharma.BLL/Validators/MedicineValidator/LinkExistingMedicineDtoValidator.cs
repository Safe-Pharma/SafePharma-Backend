using FluentValidation;

namespace SafePharma.BLL
{
    public class LinkExistingMedicineDtoValidator : AbstractValidator<LinkExistingMedicineDto>
    {
        public LinkExistingMedicineDtoValidator()
        {
            RuleFor(x => x.MedicineId).NotEmpty();
            RuleFor(x => x.TaxId).NotEmpty();
            RuleFor(x => x.PurchasePrice).GreaterThanOrEqualTo(0);
            RuleFor(x => x.SellingPrice).GreaterThanOrEqualTo(0);
            RuleFor(x => x.MinStockLevel).GreaterThanOrEqualTo(0);
        }
    }
}