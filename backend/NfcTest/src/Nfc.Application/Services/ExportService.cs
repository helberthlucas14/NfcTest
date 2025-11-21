using Nfc.Application.Export;
using Nfc.Application.Export.Interfaces;
using Nfc.Domain.Entity;
using Nfc.Domain.Interfaces.Services;

namespace Nfc.Application.Services
{
    public class ExportNotasFiscalService : IExportNotasFiscalService
    {
        private readonly IExportFactory _factory;
        private readonly INotaFiscalService _service;
        public ExportNotasFiscalService(
            IExportFactory factory,
            INotaFiscalService service)
        {
            _factory = factory;
            _service = service;
        }

        public async Task<byte[]> ExportAsync(
            ExportType type,
            IList<long> ids,
            CancellationToken cancellationToken)
        {
            var exporter = _factory.Create(type);
            var notas = new List<NotaFiscal>();
            if (ids != null && ids.Count > 0)
                notas = _service.GetAllQuery.ToList()
                    .Where(n => ids.Contains(n.Id))
                    .ToList();

            return await exporter.ExportAsync(notas, cancellationToken);
        }
    }


}
