using FluentValidation;

namespace SafePharma.BLL
{
    public class UpdateLanguageDtoValidator : AbstractValidator<UpdateLanguageDto>
    {
        public UpdateLanguageDtoValidator()
        {
            RuleFor(x => x.Language)
                .NotEmpty()
                .Must(lang => lang == "en" || lang == "ar")
                .WithMessage("Language must be 'en' or 'ar'");
        }
    }
}
