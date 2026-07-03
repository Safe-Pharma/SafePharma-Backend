using FluentValidation;

namespace SafePharma.BLL
{
    public class PrimaryContactDtoValidator : AbstractValidator<PrimaryContactDto>
    {
        public PrimaryContactDtoValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full name is required.")
                .MaximumLength(100);

            RuleFor(x => x.Mobile)
                .NotEmpty().WithMessage("Mobile is required.")
                .Matches(@"^[0-9+\-\s]{7,20}$").WithMessage("Mobile must be a valid number.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email must be valid.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters.");
        }
    }
}