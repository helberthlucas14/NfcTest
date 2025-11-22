namespace Nfc.Application.Exceptions
{
    public class NotFoundException : ApplicationException
    {
        public NotFoundException(string? message) : base(message)
        {
        }

        public static void ThrowIfNull(
            object? @object,
            string exceptionMessage)
        {
            if (@object is null)
                throw new NotFoundException(exceptionMessage);
        }

        public static void ThrowIfCondition(
           bool condition,
           string exceptionMessage)
        {
            if (condition)
                throw new NotFoundException(exceptionMessage);
        }
    }
}
