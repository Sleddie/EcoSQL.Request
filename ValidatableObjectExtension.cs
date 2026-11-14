using System.ComponentModel.DataAnnotations;

namespace EcoSQL.Request;

public static class ValidatableObjectExtension
{
    extension(IValidatableObject validatableObject)
    {
        public bool IsValid(IMessageUnit? messageUnit = null)
        {
            IEnumerable<ValidationResult> errors =
                validatableObject.Validate(new(validatableObject));

            if (errors.Any())
            {
                foreach (var error in errors)
                {
                    messageUnit?.Message = error.ErrorMessage;
                }

                return false;
            }

            return true;
        }
    }
}