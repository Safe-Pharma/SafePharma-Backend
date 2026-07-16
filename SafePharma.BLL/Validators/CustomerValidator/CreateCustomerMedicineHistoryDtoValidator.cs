using FluentValidation;

namespace SafePharma.BLL
{
    public class CreateCustomerMedicineHistoryDtoValidator : AbstractValidator<CreateCustomerMedicineHistoryDto>
    {
        public CreateCustomerMedicineHistoryDtoValidator()
        {
            RuleFor(x => x.ScientificName)
                .NotEmpty()
                .WithMessage("Scientific name is required when the medicine isn't in the global catalog.")
                .When(x => x.MedicineId is null);

            RuleFor(x => x.ScientificName)
                .MaximumLength(255);

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than zero.");

            RuleFor(x => x.Notes)
                .MaximumLength(1000);
        }
    }
}
