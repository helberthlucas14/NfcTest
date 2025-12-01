using Hangfire;
using Nfc.Application.Export;
using Nfc.Application.Logging;
using Nfc.Application.Services;
using StackExchange.Redis;
using System.Linq;
using Serilog.Context;
using System.Diagnostics;
using Nfc.Infra.Observability;

namespace Nfc.Infra.HangFire.Jobs
{
    public class ExportScheduler : IExportScheduler
    {
        private readonly ICorrelationContext _ctx;
        private readonly IApplicationLogging _logger;
        private readonly IExportStatusNotifier _notifier;
        private readonly IExportStatusRepository _repository;
        private readonly IConnectionMultiplexer _connection;
        private static readonly TimeSpan DedupExpiry = TimeSpan.FromHours(1);
        private static readonly TimeSpan LockTimeout = TimeSpan.FromSeconds(5);

        public ExportScheduler(ICorrelationContext ctx, IApplicationLogging logger, IExportStatusNotifier notifier, IExportStatusRepository repository, IConnectionMultiplexer connection)
        {
            _ctx = ctx;
            _logger = logger;
            _notifier = notifier;
            _repository = repository;
            _connection = connection;
        }

        public async Task<string> ScheduleExportAsync(ExportType type, long[] ids, CancellationToken cancellationToken)
        {
            var correlationId = _ctx.CorrelationId != Guid.Empty ? _ctx.CorrelationId : Guid.NewGuid();
            _ctx.CorrelationId = correlationId;
            using var activity = ObservabilitySetup.Activity.StartActivity("hangfire.schedule", ActivityKind.Producer);
            activity?.SetTag("export.type", type.ToString());
            activity?.SetTag("export.ids.count", ids?.Length ?? 0);
            activity?.SetTag("correlation.id", correlationId);
            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                _logger.LogStarted(correlationId, nameof(ScheduleExportAsync));
            }
            var db = _connection.GetDatabase();
            var dedupKey = BuildDedupKey(type, ids);
            var lockKey = dedupKey + ":lock";
            var token = Guid.NewGuid().ToString("N");

            var existingBeforeLock = await TryGetExistingJobIdAsync(db, dedupKey);
            if (existingBeforeLock is not null)
            {
                _ctx.JobId = existingBeforeLock;
                using (LogContext.PushProperty("CorrelationId", correlationId))
                using (LogContext.PushProperty("JobId", existingBeforeLock))
                {
                    _logger.LogCompleted(correlationId, nameof(ScheduleExportAsync), 0, existingBeforeLock);
                }
                return existingBeforeLock;
            }

            var lockTaken = await db.LockTakeAsync(lockKey, token, LockTimeout);
            if (!lockTaken)
            {
                var existing = await TryGetExistingJobIdAsync(db, dedupKey);
                if (existing is not null)
                {
                    _ctx.JobId = existing;
                    using (LogContext.PushProperty("CorrelationId", correlationId))
                    using (LogContext.PushProperty("JobId", existing))
                    {
                        _logger.LogCompleted(correlationId, nameof(ScheduleExportAsync), 0, existing);
                    }
                    return existing;
                }
            }
            try
            {
                var existing = await TryGetExistingJobIdAsync(db, dedupKey);
                if (existing is not null)
                {
                    _ctx.JobId = existing;
                    using (LogContext.PushProperty("CorrelationId", correlationId))
                    using (LogContext.PushProperty("JobId", existing))
                    {
                        _logger.LogCompleted(correlationId, nameof(ScheduleExportAsync), 0, existing);
                    }
                    return existing;
                }

                var jobId = BackgroundJob.Enqueue<ExportJob>(
                    job => job.ExecutarAsync(type, ids, correlationId, null!, cancellationToken)
                );

                await db.StringSetAsync(dedupKey, jobId, DedupExpiry, When.NotExists);

                _ctx.JobId = jobId;
                using (LogContext.PushProperty("CorrelationId", correlationId))
                using (LogContext.PushProperty("JobId", jobId))
                {
                    _logger.LogCompleted(correlationId, nameof(ScheduleExportAsync), 0, jobId);
                }

                var status = new ExportStatus
                {
                    JobId = jobId,
                    CorrelationId = correlationId,
                    State = ExportJobState.Queued,
                    Type = type,
                    Ids = ids
                };
                await _repository.SaveAsync(status, cancellationToken);
                await _notifier.NotifyAsync(status, cancellationToken);

                return jobId;
            }
            finally
            {
                if (lockTaken)
                {
                    await db.LockReleaseAsync(lockKey, token);
                }
            }
        }

        private static string BuildDedupKey(ExportType type, long[] ids)
        {
            var normalized = string.Join('-', ids.OrderBy(x => x));
            return $"export:dedup:{type}:{normalized}";
        }

        private static async Task<string?> TryGetExistingJobIdAsync(IDatabase db, string dedupKey)
        {
            var value = await db.StringGetAsync(dedupKey);
            return value.HasValue ? value.ToString() : null;
        }
    }
}
