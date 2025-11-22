namespace Nfc.Application.Export
{
    public enum ExportJobState
    {
        Queued,
        Started,
        Completed,
        Failed
    }

    public class ExportStatus
    {
        public required string JobId { get; init; }
        public Guid CorrelationId { get; init; }
        public ExportJobState State { get; init; }
        public ExportType Type { get; init; }
        public required long[] Ids { get; init; }
        public double? DurationMs { get; init; }
        public string? Error { get; init; }
        public string? FileUrl { get; init; }
        public DateTime OccurredAtUtc { get; init; } = DateTime.UtcNow;
    }
}