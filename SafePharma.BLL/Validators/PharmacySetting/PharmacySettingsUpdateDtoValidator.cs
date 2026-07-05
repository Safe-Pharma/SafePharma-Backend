using FluentValidation;

namespace SafePharma.BLL
{
    public class PharmacySettingsUpdateDtoValidator : AbstractValidator<PharmacySettingsUpdateDto>
    {
        public PharmacySettingsUpdateDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

            RuleFor(x => x.LogoFile)
                .Must(file =>
                    file == null ||
                    file.ContentType.StartsWith("image/"))
                .WithMessage("Only image files are allowed.");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Street is required.")
                .MaximumLength(300);

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("City is required.")
                .MaximumLength(100);

            RuleFor(x => x.Country)
                .NotEmpty().WithMessage("Governorate is required.")
                .MaximumLength(100);

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Phone is required.")
                .Matches(@"^[0-9+\-\s]{7,20}$")
                .WithMessage("Phone must be a valid number.");

            RuleFor(x => x.TaxRegistrationNumber)
                .NotEmpty().WithMessage("TaxRegistrationNumber is required.");
        }
    }
}