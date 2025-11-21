namespace Nfc.Application.Logging
{
    public interface IApplicationLogging
    {
        void LogStarted(Guid correlationId, string operation, string? jobId = null);
        void LogCompleted(Guid correlationId, string operation, double durationMs, string? jobId = null);
        void LogFailure(Guid correlationId, string operation, double durationMs, Exception ex, string? jobId = null);
    }
}
