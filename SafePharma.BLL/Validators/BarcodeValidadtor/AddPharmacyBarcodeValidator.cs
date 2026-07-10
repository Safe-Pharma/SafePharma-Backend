using FluentValidation;

public class AddPharmacyBarcodeValidator
    : AbstractValidator<AddPharmacyBarcodeDto>
{
    public AddPharmacyBarcodeValidator()
    {
        RuleFor(x => x.PharmacyMedicineId)
            .NotEmpty();

        RuleFor(x => x.Barcode)
            .MaximumLength(100);
    }
}