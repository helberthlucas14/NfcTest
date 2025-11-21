using FC.Codeflix.Catalog.Application.Exceptions;
using Nfc.Application.Export.Interfaces;

namespace Nfc.Application.Export
{
    public class ExportFactory : IExportFactory
    {
        private readonly IEnumerable<IExporter> _exporters;
        public ExportFactory(IEnumerable<IExporter> exporters)
        {
            _exporters = exporters;
        }
        public IExporter Create(ExportType type)
        {
            var exporter = _exporters.FirstOrDefault(e => e.Type == type);
            ExportException.ThrowIfNull(exporter, $"Export type '{type}' is not supported.");
            return exporter!;
        }
    }
}
