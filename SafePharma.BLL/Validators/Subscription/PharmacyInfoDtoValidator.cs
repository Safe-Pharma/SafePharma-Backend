using FluentValidation;

namespace SafePharma.BLL
{
    public class PharmacyInfoDtoValidator : AbstractValidator<PharmacyInfoDto>
    {
        public PharmacyInfoDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Pharmacy name is required.")
                .MaximumLength(100);

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Address is required.")
                .MaximumLength(200);

            RuleFor(x => x.Country)
                .NotEmpty().WithMessage("Country is required.");

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("City is required.");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Phone is required.")
                .Matches(@"^[0-9+\-\s]{7,20}$").WithMessage("Phone must be a valid number.");

            RuleFor(x => x.BusinessEmail)
                .NotEmpty().WithMessage("Business email is required.")
                .EmailAddress().WithMessage("Business email must be valid.");

            RuleFor(x => x.NumberOfBranches)
                .GreaterThan(0).WithMessage("Number of branches must be at least 1.");

            RuleFor(x => x.PreferredLanguage)
                .NotEmpty().WithMessage("Preferred language is required.");

            RuleFor(x => x.TimeZone)
                .NotEmpty().WithMessage("Time zone is required.");
        }
    }
}