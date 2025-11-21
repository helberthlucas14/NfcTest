using MediatR;
using Nfc.Application.Export.Interfaces;
using Nfc.Application.Services;

namespace Nfc.Application.UseCases.ExportNotaFiscal
{
    public class ExportNotaFiscalCommandHandler : IRequestHandler<ExportNotaFiscalCommand, long>
    {
        private readonly IExportScheduler _exportScheduler;
        private readonly IExportService _service;
        private readonly IExportNotasFiscalService _export;
        public ExportNotaFiscalCommandHandler(IExportScheduler exportScheduler, IExportNotasFiscalService export, IExportService service)
        {
            _exportScheduler = exportScheduler;
            _export = export;
            _service = service;
        }

        public override async Task<long> Handle(ExportNotaFiscalCommand request, CancellationToken cancellationToken)
        {
            var normalized = await _export.ValidateAndNormalizeAsync(request.Ids, request.Type, cancellationToken);
            var jobId = await _service.StartExportAsync(normalized.NoteIds.ToString, normalized.Format, request.CorrelationId, cancellationToken);
            await _exportScheduler.SheculerExportAsync(request.Type, cancellationToken);
            return jobId;
        }
    }
}
