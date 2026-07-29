using FluentValidation;

namespace SafePharma.BLL
{
    public class CustomerCreateDtoValidator : AbstractValidator<CustomerCreateDto>
    {
        public CustomerCreateDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Customer name is required.")
                .MaximumLength(255);

            When(x => !x.HasParent, () =>
            {
                RuleFor(x => x.Phone)
                    .NotEmpty().WithMessage("Phone is required.")
                    .MaximumLength(50);
            });


            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Enter a valid email address.")
                .MaximumLength(255)
                .When(x => !string.IsNullOrWhiteSpace(x.Email));

            RuleFor(x => x.Address)
                .MaximumLength(500);

            RuleFor(x => x.DateOfBirth)
                .LessThan(DateTime.UtcNow).WithMessage("Date of birth must be in the past.")
                .When(x => x.DateOfBirth.HasValue);

            RuleFor(x => x.Status)
                .NotEmpty()
                .Must(s => s == "Active" || s == "Inactive")
                .WithMessage("Status must be either 'Active' or 'Inactive'.");

            //RuleFor(x => x.Outstanding)
            //    .GreaterThanOrEqualTo(0).WithMessage("Outstanding balance cannot be negative.");

            RuleFor(x => x.Notes)
                .MaximumLength(1000);
        }
    }
}