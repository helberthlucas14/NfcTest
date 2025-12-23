using Nfc.Application.Exceptions;
using Nfc.Application.Export;
using Nfc.Application.Export.Interfaces;
using Nfc.Application.Logging;
using Nfc.Domain.Entity;
using Nfc.Domain.Interfaces.Services;

namespace Nfc.Application.Services
{
    public class ExportNotasFiscalService : IExportNotasFiscalService
    {
        private readonly IExportFactory _factory;
        private readonly INotaFiscalService _service;
        private readonly IApplicationLogging _logger;
        private readonly ICorrelationContext _ctx;
        public ExportNotasFiscalService(
            IExportFactory factory,
            INotaFiscalService service,
            IApplicationLogging logger,
            ICorrelationContext ctx)
        {
            _factory = factory;
            _service = service;
            _logger = logger;
            _ctx = ctx;
        }

        public async Task<ExportStartData> ValidateAndNormalizeAsync(
            long[] noteIds,
            ExportType format,
            CancellationToken cancellationToken)
        {
            _logger.LogStarted(_ctx.CorrelationId, nameof(ValidateAndNormalizeAsync), _ctx.JobId);
            var start = DateTime.UtcNow;
            try
            {
                if (noteIds == null || noteIds.Length == 0)
                    throw new ExportException("Ids not be null or empty.");

                foreach (var id in noteIds)
                {
                    var exists = await _service.ExistsAsync(id, cancellationToken);
                    NotFoundException.ThrowIfCondition(!exists, $"Nota Fiscal '{id}' not found.");
                }

                var duration = (DateTime.UtcNow - start).TotalMilliseconds;
                _logger.LogCompleted(_ctx.CorrelationId, nameof(ValidateAndNormalizeAsync), duration, _ctx.JobId);
                return new ExportStartData(noteIds, format);
            }
            catch (Exception ex)
            {
                var duration = (DateTime.UtcNow - start).TotalMilliseconds;
                _logger.LogFailure(_ctx.CorrelationId, nameof(ValidateAndNormalizeAsync), duration, ex, _ctx.JobId);
                throw;
            }
        }

        public async Task<byte[]> ExportAsync(
            ExportType type,
            IList<long> ids,
            CancellationToken cancellationToken)
        {
            _logger.LogStarted(_ctx.CorrelationId, nameof(ExportAsync), _ctx.JobId);
            var start = DateTime.UtcNow;
            try
            {
                var exporter = _factory.Create(type);
                var notas = new List<NotaFiscal>();
                if (ids != null && ids.Count > 0)
                {
                    var result = await _service.GetListByIdsAsync(ids, cancellationToken);
                    notas = result.ToList();
                }

                var bytes = await exporter.ExportAsync(notas, cancellationToken);
                var duration = (DateTime.UtcNow - start).TotalMilliseconds;
                _logger.LogCompleted(_ctx.CorrelationId, nameof(ExportAsync), duration, _ctx.JobId);
                return bytes;
            }
            catch (Exception ex)
            {
                var duration = (DateTime.UtcNow - start).TotalMilliseconds;
                _logger.LogFailure(_ctx.CorrelationId, nameof(ExportAsync), duration, ex, _ctx.JobId);
                throw;
            }
        }
    }


}
