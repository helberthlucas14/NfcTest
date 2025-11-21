namespace Nfc.Application.Logging
{
    public class CorrelationContext : ICorrelationContext
    {
        public Guid CorrelationId { get; set; } = Guid.Empty;
        public string? JobId { get; set; }
    }
}
