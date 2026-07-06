using FluentValidation;

namespace SafePharma.BLL
{
    public class SupplierUpdateDtoValidator : AbstractValidator<SupplierUpdateDto>
    {
        public SupplierUpdateDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Supplier name is required.")
                .MaximumLength(255);

            RuleFor(x => x.ContactPerson)
                .NotEmpty().WithMessage("Contact person is required.")
                .MaximumLength(255);

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Phone is required.")
                .MaximumLength(50);

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Enter a valid email address.")
                .MaximumLength(255);

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Address is required.");

            RuleFor(x => x.CountryId)
                .NotEmpty().WithMessage("Country is required.");

            RuleFor(x => x.Status)
                .NotEmpty()
                .Must(s => s == "Active" || s == "Inactive")
                .WithMessage("Status must be either 'Active' or 'Inactive'.");

            RuleFor(x => x.Outstanding)
                .GreaterThanOrEqualTo(0).WithMessage("Outstanding balance cannot be negative.");

            RuleFor(x => x.TaxNumber)
                .MaximumLength(100);
        }
    }
}
