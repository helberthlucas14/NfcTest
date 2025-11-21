namespace Nfc.Application.Logging
{
    public interface ICorrelationContext
    {
        Guid CorrelationId { get; set; }
    }
}
