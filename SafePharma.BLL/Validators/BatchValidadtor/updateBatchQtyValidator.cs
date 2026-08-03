using FluentValidation;
using SafePharma.BLL;

namespace SafePharma.BLL.Validators
{
    public class UpdateBatchQtyValidator : AbstractValidator<BatchQtyDto>
    {
        public UpdateBatchQtyValidator()
        {
            RuleFor(x => x.BatchId)
                .NotEmpty()
                .WithMessage("Batch ID is required.");

            RuleFor(x => x.NewStock)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Stock must not be negative.");
        }
    }
}