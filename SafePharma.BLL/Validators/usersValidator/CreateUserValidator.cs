using FluentValidation;

namespace SafePharma.BLL
{

    public class CreateUserValidator : AbstractValidator<CreateUserRequest>
    {
        public CreateUserValidator()
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

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required")
                .MinimumLength(8)
                .WithMessage("Password must be at least 8 characters")
                .Matches("[A-Z]")
                .WithMessage("Password must contain at least one uppercase letter")
                .Matches("[a-z]")
                .WithMessage("Password must contain at least one lowercase letter")
                .Matches("[0-9]")
                .WithMessage("Password must contain at least one number")
                .Matches("[^a-zA-Z0-9]")
                .WithMessage("Password must contain at least one special character");

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty()
                .WithMessage("Confirm password is required")
                .Equal(x => x.Password)
                .WithMessage("Passwords do not match");

            RuleFor(x => x.Role)
                .NotEmpty()
                .WithMessage("Role is required");

            RuleFor(x => x.Branch)
                .MaximumLength(100)
                .WithMessage("Branch cannot exceed 100 characters");

            RuleFor(x => x.IsActive)
                .NotNull();
        }
    }
}
