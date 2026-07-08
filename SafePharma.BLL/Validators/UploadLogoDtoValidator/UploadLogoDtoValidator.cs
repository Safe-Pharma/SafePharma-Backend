using FluentValidation;

namespace SafePharma.BLL
{
    public class UploadLogoDtoValidator : AbstractValidator<UploadLogoDto>
    {
        private static readonly string[] AllowedLogoContentTypes = { "image/jpeg", "image/png", "image/svg+xml" };
        private const long MaxLogoSizeBytes = 5 * 1024 * 1024;

        public UploadLogoDtoValidator()
        {
            RuleFor(x => x.Logo)
                .NotNull().WithMessage("A logo file is required.");

            RuleFor(x => x.Logo.Length)
                .LessThanOrEqualTo(MaxLogoSizeBytes).WithMessage("Logo must be under 5MB.")
                .When(x => x.Logo != null);

            RuleFor(x => x.Logo.ContentType)
                .Must(t => AllowedLogoContentTypes.Contains(t)).WithMessage("Logo must be a PNG, JPG, or SVG.")
                .When(x => x.Logo != null);
        }
    }
}