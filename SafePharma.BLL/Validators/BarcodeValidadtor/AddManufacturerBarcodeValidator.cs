using FluentValidation;

public class AddManufacturerBarcodeValidator
    : AbstractValidator<AddManufacturerBarcodeDto>
{
    public AddManufacturerBarcodeValidator()
    {

        RuleFor(x => x).NotNull().WithMessage("Request body cannot be null");
        RuleFor(x => x.MedicineId)
            .NotEmpty();

        RuleFor(x => x.Barcode)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.IsPrimary)
            .NotNull();
    }
}