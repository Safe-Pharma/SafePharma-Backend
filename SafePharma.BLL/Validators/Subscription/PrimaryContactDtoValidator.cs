using FluentValidation;
using System.Text.RegularExpressions;

namespace SafePharma.BLL
{
    public class PrimaryContactDtoValidator : AbstractValidator<PrimaryContactDto>
    {
        public PrimaryContactDtoValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full name is required.")
                .Length(2, 100).WithMessage("Full name must be between 2 and 100 characters.")
                .Matches(@"^[a-zA-Z\u0600-\u06FF\s'\-]+$").WithMessage("Full name can only contain letters, spaces, hyphens, or apostrophes.");

            RuleFor(x => x.Mobile)
                .NotEmpty().WithMessage("Mobile is required.")
                .Must(BeAValidPhoneNumber).WithMessage("Mobile must be a valid international number (e.g. +971501234567).");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email must be valid.")
                .MaximumLength(150);

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
                .MaximumLength(100).WithMessage("Password must not exceed 100 characters.")
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches("[0-9]").WithMessage("Password must contain at least one digit.")
                .Matches(@"[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");
        }

        private bool BeAValidPhoneNumber(string mobile)
        {
            if (string.IsNullOrWhiteSpace(mobile)) return false;
            var digitsOnly = Regex.Replace(mobile, @"[^\d+]", "");
            return Regex.IsMatch(digitsOnly, @"^\+[1-9]\d{7,14}$");
        }
    }
}