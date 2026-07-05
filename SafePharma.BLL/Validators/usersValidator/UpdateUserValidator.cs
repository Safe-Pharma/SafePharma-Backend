using FluentValidation;

namespace SafePharma.BLL
{
    public class UpdateUserValidator : AbstractValidator<UpdateUserRequest>
    {
        public UpdateUserValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage("First name is required")
                .Length(2, 50)
                .WithMessage("First name must be between 2 and 50 characters");

            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage("Last name is required")
                .Length(2, 50)
                .WithMessage("Last name must be between 2 and 50 characters");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required")
                .EmailAddress()
                .WithMessage("Invalid email address");

            RuleFor(x => x.Phone)
                .NotEmpty()
                .WithMessage("Phone number is required")
                .Matches(@"^01[0125][0-9]{8}$")
                .WithMessage("Invalid Egyptian phone number");

            RuleFor(x => x.Role)
                .NotEmpty()
                .WithMessage("Role is required");

            RuleFor(x => x.Branch)
                .MaximumLength(100)
                .WithMessage("Branch cannot exceed 100 characters");
        }

    }
}
