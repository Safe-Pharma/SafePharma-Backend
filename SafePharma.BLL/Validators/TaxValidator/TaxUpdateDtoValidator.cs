using FluentValidation;

namespace SafePharma.BLL
{
    public class TaxUpdateDtoValidator : AbstractValidator<TaxUpdateDto>
    {
        public TaxUpdateDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Tax name is required.")
                .MaximumLength(100).WithMessage("Tax name must not exceed 100 characters.");

            RuleFor(x => x.Rate)
                .InclusiveBetween(0, 100).WithMessage("Tax rate must be between 0 and 100.");

            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Status is required.")
                .Must(s => s == "Active" || s == "Inactive")
                .WithMessage("Status must be either 'Active' or 'Inactive'.");
        }
    }
}
