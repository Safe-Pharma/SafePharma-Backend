using FluentValidation;
using System.Text.RegularExpressions;

namespace SafePharma.BLL
{
    public class PharmacyInfoDtoValidator : AbstractValidator<PharmacyInfoDto>
    {
        private static readonly string[] ValidLanguages = { "English", "Arabic" };
        private static readonly string[] ValidTimeZones =
        {
            "(GMT+4) Gulf Standard Time",
            "(GMT+3) Arabia Standard Time",
            "(GMT+2) Eastern European Time"
        };

        public PharmacyInfoDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Pharmacy name is required.")
                .Length(2, 100).WithMessage("Pharmacy name must be between 2 and 100 characters.");

            RuleFor(x => x.LogoUrl)
                .Must(BeAValidUrl).WithMessage("Pharmacy logo must be a valid URL.")
                .When(x => !string.IsNullOrWhiteSpace(x.LogoUrl));

            RuleFor(x => x.TaxNumber)
                .Matches(@"^[A-Za-z0-9\-]{5,20}$").WithMessage("Tax number must be 5–20 characters, letters, numbers, or hyphens only.")
                .When(x => !string.IsNullOrWhiteSpace(x.TaxNumber));

            RuleFor(x => x.CommercialRegistration)
                .Matches(@"^[A-Za-z0-9\-]{5,20}$").WithMessage("Commercial registration must be 5–20 characters, letters, numbers, or hyphens only.")
                .When(x => !string.IsNullOrWhiteSpace(x.CommercialRegistration));

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Address is required.")
                .Length(5, 200).WithMessage("Address must be between 5 and 200 characters.");

            RuleFor(x => x.Country)
                .NotEmpty().WithMessage("Country is required.")
                .MaximumLength(100);

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("City is required.")
                .MaximumLength(100);

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Phone is required.")
                .Must(BeAValidPhoneNumber).WithMessage("Phone must be a valid international number (e.g. +971501234567).");

            RuleFor(x => x.BusinessEmail)
                .NotEmpty().WithMessage("Business email is required.")
                .EmailAddress().WithMessage("Business email must be valid.")
                .MaximumLength(150);

            RuleFor(x => x.NumberOfBranches)
                .GreaterThan(0).WithMessage("Number of branches must be at least 1.")
                .LessThanOrEqualTo(1000).WithMessage("Number of branches seems unreasonably high — please contact support for enterprise onboarding.");

            RuleFor(x => x.PreferredLanguage)
                .NotEmpty().WithMessage("Preferred language is required.")
                .Must(l => ValidLanguages.Contains(l)).WithMessage($"Preferred language must be one of: {string.Join(", ", ValidLanguages)}.");

            RuleFor(x => x.TimeZone)
                .NotEmpty().WithMessage("Time zone is required.")
                .Must(t => ValidTimeZones.Contains(t)).WithMessage("Please select a valid time zone from the list.");
        }

        private bool BeAValidPhoneNumber(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return false;
            var digitsOnly = Regex.Replace(phone, @"[^\d+]", "");
            return Regex.IsMatch(digitsOnly, @"^\+[1-9]\d{7,14}$");
        }

        private bool BeAValidUrl(string? url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out var result)
                && (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
        }
    }
}