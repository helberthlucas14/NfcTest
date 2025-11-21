namespace FC.Codeflix.Catalog.Application.Exceptions
{
    public class ExportException : ApplicationException
    {
        public ExportException(string? message) : base(message)
        {
        }

        public static void ThrowIfNull(object? @object, string exceptionMessage)
        {
            if (@object is null)
                throw new NotFoundException(exceptionMessage);
        }
    }



}
