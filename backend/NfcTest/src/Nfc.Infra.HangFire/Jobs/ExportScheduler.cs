using Hangfire;
using Nfc.Application.Export;
using Nfc.Application.Logging;
using Nfc.Application.Services;

namespace Nfc.Infra.HangFire.Jobs
{
    public class ExportScheduler : IExportScheduler
    {
        private readonly ICorrelationContext _ctx;
        private readonly IApplicationLogging _logger;
        private readonly IExportStatusNotifier _notifier;
        private readonly IExportStatusRepository _repository;

        public ExportScheduler(ICorrelationContext ctx, IApplicationLogging logger, IExportStatusNotifier notifier, IExportStatusRepository repository)
        {
            _ctx = ctx;
            _logger = logger;
            _notifier = notifier;
            _repository = repository;
        }

        public async Task<string> ScheduleExportAsync(ExportType type, long[] ids, CancellationToken cancellationToken)
        {
            var correlationId = Guid.NewGuid();
            _ctx.CorrelationId = correlationId;

            _logger.LogStarted(correlationId, nameof(ScheduleExportAsync));

            var jobId = BackgroundJob.Enqueue<ExportJob>(
                job => job.ExecutarAsync(type, ids, correlationId, null!, cancellationToken)
            );

            _ctx.JobId = jobId;
            _logger.LogCompleted(correlationId, nameof(ScheduleExportAsync), 0, jobId);

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

            return await Task.FromResult(jobId);
        }
    }
}
