using MediatR;
using Nfc.Application.Services;

namespace Nfc.Application.UseCases.ExportNotaFiscal
{
    public class ExportNotaFiscalCommandHandler : IRequestHandler<ExportNotaFiscalCommand, string>
    {
        private readonly IExportScheduler _exportScheduler;
        private readonly IExportNotasFiscalService _export;
        public ExportNotaFiscalCommandHandler(IExportScheduler exportScheduler, IExportNotasFiscalService export)
        {
            _exportScheduler = exportScheduler;
            _export = export;
        }

        public async Task<string> Handle(ExportNotaFiscalCommand request, CancellationToken cancellationToken)
        {
            var normalized = await _export.ValidateAndNormalizeAsync(request.Ids, request.Type, cancellationToken);
            var jobId = await _exportScheduler.ScheduleExportAsync(normalized.Format, normalized.NoteIds, cancellationToken);
            request.JobId = jobId;
            return jobId;
        }
    }
}
