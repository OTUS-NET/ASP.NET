using DirectoryOfPreferences.Model.Request;
using FluentValidation;

namespace DirectoryOfPreferences.Validators
{
    public class CreatePreferenceValidator: AbstractValidator<PreferenceRequest>
    {
        public CreatePreferenceValidator()
        {
            RuleFor(x => x.Name)
               .NotNull()
               .NotEmpty()
            .MaximumLength(50)
            .Must(ValidateName)
            .WithMessage("Name must contain only letters or whitespaces");
        }

        internal static bool ValidateName(string name)
        { 
            if (name.Any(c => !(char.IsLetter(c) || char.IsWhiteSpace(c))))
                return false;
            return true;
        }
    }
}
