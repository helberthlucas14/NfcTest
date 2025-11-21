using Nfc.Application.Export.Interfaces;

namespace Nfc.Application.Export.Exporters
{

    public class TextExporter : IExporter
    {
        public ExportType Type => ExportType.TXT;

        public Task<byte[]> ExportAsync<T>(IEnumerable<T> data, CancellationToken cancellationToken)
        {
            var text = data?.ToString() ?? string.Empty;
            return Task.FromResult(System.Text.Encoding.UTF8.GetBytes(text));
        }
    }
}
