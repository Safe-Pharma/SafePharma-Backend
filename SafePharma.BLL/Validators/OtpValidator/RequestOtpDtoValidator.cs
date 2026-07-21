using FluentValidation;

namespace SafePharma.BLL
{
    public class RequestOtpDtoValidator : AbstractValidator<RequestOtpDto>
    {
        public RequestOtpDtoValidator()
        {
            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Phone is required.")
                .Matches(@"^\+?[0-9]{10,15}$").WithMessage("Enter a valid phone number.");
        }
    }
}