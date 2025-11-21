using Hangfire;
using Nfc.Application.Export;
using Nfc.Application.Logging;
using Nfc.Application.Services;
using System.Threading;

namespace Nfc.Infra.HangFire.Jobs
{
    public class ExportacaoScheduler
    {
        private readonly ICorrelationContext _ctx;
        private readonly IApplicationLogging _logger;

        public ExportacaoScheduler(ICorrelationContext ctx, IApplicationLogging logger)
        {
            _ctx = ctx;
            _logger = logger;
        }

        public string SheculerExportAsync(ExportType type, CancellationToken cancellationToken)
        {
            var correlationId = Guid.NewGuid();
            _ctx.CorrelationId = correlationId;

            _logger.LogStarted(correlationId, "AgendarExportacao");

            var jobId = BackgroundJob.Enqueue<ExportJob>(
                job => job.ExecutarAsync(type, cancellationToken)
            );

            _ctx.JobId = jobId;

            _logger.LogCompleted(correlationId, "AgendarExportacao", 0, jobId);

            return jobId;
        }
    }

    public class ExportJob
    {
        private readonly IExportNotasFiscalService _service;

        public ExportJob(IExportNotasFiscalService service)
        {
            _service = service;
        }

        public async Task ExecutarAsync(ExportType type, CancellationToken cancellationToken)
        {
            var bytes = await _service.ExportAsync(type, cancellationToken);
        }
    }
}
