using System.ComponentModel.DataAnnotations;

namespace EcoSQL.Request;

/// <summary>Provides extension methods for validating objects that implement <see cref="IValidatableObject"/>.</summary>
public static class ValidatableObjectExtension
{
    /// <param name="validatableObject">The object to validate.</param>
    extension(IValidatableObject validatableObject)
    {
        /// <summary>Validates the object and optionally sets a message in the provided <see cref="IMessageUnit"/>.</summary>
        /// <param name="messageUnit">The message unit to set the validation message in.</param>
        /// <returns>True if the object is valid; otherwise, false.</returns>
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