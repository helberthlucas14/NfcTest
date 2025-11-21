namespace Nfc.Application.Logging
{
    public class CorrelationContext : ICorrelationContext
    {
        public Guid CorrelationId { get; set; } = Guid.Empty;
    }
}
