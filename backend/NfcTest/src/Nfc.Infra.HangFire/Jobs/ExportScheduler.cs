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

        public ExportScheduler(ICorrelationContext ctx, IApplicationLogging logger)
        {
            _ctx = ctx;
            _logger = logger;
        }

        public async Task<string> SheculerExportAsync(ExportType type, CancellationToken cancellationToken)
        {
            var correlationId = Guid.NewGuid();
            _ctx.CorrelationId = correlationId;

            _logger.LogStarted(correlationId, nameof(SheculerExportAsync));

            var jobId = BackgroundJob.Enqueue<ExportJob>(
                job => job.ExecutarAsync(type, cancellationToken)
            );

            _ctx.JobId = jobId;
            _logger.LogCompleted(correlationId, nameof(SheculerExportAsync), 0, jobId);

            return await Task.FromResult(jobId);
        }
    }
}
