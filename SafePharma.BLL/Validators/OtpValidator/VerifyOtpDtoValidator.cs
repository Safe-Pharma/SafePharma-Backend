using FluentValidation;

namespace SafePharma.BLL
{
    public class VerifyOtpDtoValidator : AbstractValidator<VerifyOtpDto>
    {
        public VerifyOtpDtoValidator()
        {
            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Phone is required.")
                .Matches(@"^\+?[0-9]{10,15}$").WithMessage("Enter a valid phone number.");

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Code is required.")
                .Length(6).WithMessage("Code must be 6 digits.")
                .Matches(@"^\d{6}$").WithMessage("Code must contain only digits.");
        }
    }
}