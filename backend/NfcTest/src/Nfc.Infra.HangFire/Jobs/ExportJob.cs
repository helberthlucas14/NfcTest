using Nfc.Application.Export;
using Nfc.Application.Logging;
using Nfc.Application.Services;

namespace Nfc.Infra.HangFire.Jobs
{
    public class ExportJob
    {
        private readonly IExportNotasFiscalService _service;
        private readonly IApplicationLogging _logger;
        private readonly ICorrelationContext _ctx;
        private readonly IExportStatusNotifier _notifier;
        private readonly IExportStatusRepository _repository;

        public ExportJob(
            IExportNotasFiscalService service,
            IApplicationLogging logger,
            ICorrelationContext ctx,
            IExportStatusNotifier notifier,
            IExportStatusRepository repository)
        {
            _service = service;
            _logger = logger;
            _ctx = ctx;
            _notifier = notifier;
            _repository = repository;
        }

        public async Task ExecutarAsync(ExportType type, long[] ids, Guid correlationId, Hangfire.Server.PerformContext context, CancellationToken cancellationToken)
        {
            _ctx.CorrelationId = correlationId;
            _ctx.JobId = context?.BackgroundJob?.Id;
            var start = DateTime.UtcNow;
            _logger.LogStarted(correlationId, nameof(ExecutarAsync), _ctx.JobId);
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


                var duration = (DateTime.UtcNow - start).TotalMilliseconds;
                _logger.LogCompleted(correlationId, nameof(ExecutarAsync), duration, _ctx.JobId);
                var completedStatus = new ExportStatus
                {
                    JobId = _ctx.JobId ?? string.Empty,
                    CorrelationId = correlationId,
                    State = ExportJobState.Completed,
                    Type = type,
                    Ids = ids,
                    DurationMs = duration
                };
                await _repository.SaveAsync(completedStatus, cancellationToken);
                await _notifier.NotifyAsync(completedStatus, cancellationToken);
            }
            catch (Exception ex)
            {
                var duration = (DateTime.UtcNow - start).TotalMilliseconds;
                _logger.LogFailure(correlationId, nameof(ExecutarAsync), duration, ex, _ctx.JobId);
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
                throw;
            }
        }
    }
}
