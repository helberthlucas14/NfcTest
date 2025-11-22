using Nfc.Application.Export;
using Nfc.Application.Logging;
using Nfc.Application.Services;
using Nfc.Application.Export.Interfaces;
using StackExchange.Redis;
using System.Linq;
using Serilog.Context;

namespace Nfc.Infra.HangFire.Jobs
{
    public class ExportJob
    {
        private readonly IExportNotasFiscalService _service;
        private readonly IApplicationLogging _logger;
        private readonly ICorrelationContext _ctx;
        private readonly IExportStatusNotifier _notifier;
        private readonly IExportStatusRepository _repository;
        private readonly IExportFileStorage _fileStorage;
        private readonly IConnectionMultiplexer _connection;

        public ExportJob(
            IExportNotasFiscalService service,
            IApplicationLogging logger,
            ICorrelationContext ctx,
            IExportStatusNotifier notifier,
            IExportStatusRepository repository,
            IExportFileStorage fileStorage,
            IConnectionMultiplexer connection)
        {
            _service = service;
            _logger = logger;
            _ctx = ctx;
            _notifier = notifier;
            _repository = repository;
            _fileStorage = fileStorage;
            _connection = connection;
        }

        public async Task ExecutarAsync(ExportType type, long[] ids, Guid correlationId, Hangfire.Server.PerformContext context, CancellationToken cancellationToken)
        {
            _ctx.CorrelationId = correlationId;
            _ctx.JobId = context?.BackgroundJob?.Id;
            var db = _connection.GetDatabase();
            var dedupKey = BuildDedupKey(type, ids);
            var start = DateTime.UtcNow;
            using (LogContext.PushProperty("CorrelationId", correlationId))
            using (LogContext.PushProperty("JobId", _ctx.JobId))
            {
                _logger.LogStarted(correlationId, nameof(ExecutarAsync), _ctx.JobId);
            }
            var startedStatus = new ExportStatus
            {
                JobId = _ctx.JobId ?? string.Empty,
                CorrelationId = correlationId,
                State = ExportJobState.Started,
                Type = type,
                Ids = ids
            };
            await _repository.SaveAsync(startedStatus, cancellationToken);
            await _notifier.NotifyAsync(startedStatus, cancellationToken);
            try
            {
                var bytes = await _service.ExportAsync(type, ids.ToList(), cancellationToken);
                using (var ms = new MemoryStream(bytes))
                {
                    await _fileStorage.SaveAsync(_ctx.JobId ?? string.Empty, type, ms, cancellationToken);
                }


                var duration = (DateTime.UtcNow - start).TotalMilliseconds;
                using (LogContext.PushProperty("CorrelationId", correlationId))
                using (LogContext.PushProperty("JobId", _ctx.JobId))
                {
                    _logger.LogCompleted(correlationId, nameof(ExecutarAsync), duration, _ctx.JobId);
                }
                var completedStatus = new ExportStatus
                {
                    JobId = _ctx.JobId ?? string.Empty,
                    CorrelationId = correlationId,
                    State = ExportJobState.Completed,
                    Type = type,
                    Ids = ids,
                    DurationMs = duration,
                    FileUrl = await _fileStorage.GetPublicUrlAsync(_ctx.JobId ?? string.Empty, type, cancellationToken: cancellationToken) ?? $"/api/export/file/{_ctx.JobId}"
                };
                await _repository.SaveAsync(completedStatus, cancellationToken);
                await _notifier.NotifyAsync(completedStatus, cancellationToken);
                await db.KeyDeleteAsync(dedupKey);
            }
            catch (Exception ex)
            {
                var duration = (DateTime.UtcNow - start).TotalMilliseconds;
                using (LogContext.PushProperty("CorrelationId", correlationId))
                using (LogContext.PushProperty("JobId", _ctx.JobId))
                {
                    _logger.LogFailure(correlationId, nameof(ExecutarAsync), duration, ex, _ctx.JobId);
                }
                var failedStatus = new ExportStatus
                {
                    JobId = _ctx.JobId ?? string.Empty,
                    CorrelationId = correlationId,
                    State = ExportJobState.Failed,
                    Type = type,
                    Ids = ids,
                    DurationMs = duration,
                    Error = ex.Message
                };
                await _repository.SaveAsync(failedStatus, cancellationToken);
                await _notifier.NotifyAsync(failedStatus, cancellationToken);
                await db.KeyDeleteAsync(dedupKey);
                throw;
            }
        }

        private static string BuildDedupKey(ExportType type, long[] ids)
        {
            var normalized = string.Join('-', ids.OrderBy(x => x));
            return $"export:dedup:{type}:{normalized}";
        }
    }
}
