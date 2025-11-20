using Nfc.Domain.Exceptions;

namespace Nfc.Domain.Validation
{

    public class DomainValidation
    {
        public static void NotNull(object? target, string fieldName)
        {
            if (target is null)
                throw new EntityValidationException($"{fieldName} should not be null");
        }
        public static void NotNull(DateTime? target, string fieldName)
        {
            if (target is null || DateTime.MinValue == target)
                throw new EntityValidationException($"{fieldName} should not be null");
        }

        public static void NotNullOrEmpty(string? target, string fieldName)
        {
            if (String.IsNullOrWhiteSpace(target))
                throw new EntityValidationException($"{fieldName} should not be empty or null");
        }

        public static void MinLength(string target, int minLength, string fieldName)
        {
            if (target.Length < minLength)
                throw new EntityValidationException($"{fieldName} should be less than {minLength} characters long");
        }

        public static void MaxLength(string target, int maxLength, string fieldName)
        {
            if (target.Length > maxLength)
                throw new EntityValidationException($"{fieldName} should be greater than {maxLength} characters long");
        }

        public static void InvalidAtributeMinValue(decimal target, string fieldName)
        {
            if (target < 0)
                throw new EntityValidationException($"{fieldName} should be greater than 0.");
        }
    }
}
