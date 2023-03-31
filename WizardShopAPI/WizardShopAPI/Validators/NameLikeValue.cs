using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace WizardShopAPI.Validators
{
    public class NameLikeValue:ValidationAttribute
    {
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            ValidationResult result = ValidationResult.Success;

            if (value == null)
            {
                result = new ValidationResult("Empty name like value");
            }

            String namelikeValue = (String)value;

            if (!CheckCharacters(ref namelikeValue))
            {
                result = new ValidationResult("Name like values contain only letters and some special characters (. or -)");
            }
            return result;
        }

        private bool CheckCharacters(ref string value)
        {
            foreach (char c in value)
            {
                if (!(Char.IsLetter(c) || c == '.' || c == '-' || c == ' '))
                {
                    return false;
                }
            }
            return true;
        }
        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentCulture,
            ErrorMessageString, name);
        }
    }
}
