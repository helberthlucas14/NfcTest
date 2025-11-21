using Nfc.Application.Export.Interfaces;

namespace Nfc.Application.Export.Exporters
{
    public class JsonExporter : IExporter
    {
        public ExportType Type => ExportType.JSON;
        public Task<byte[]> ExportAsync<T>(IEnumerable<T> data, CancellationToken cancellationToken)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(data);
            return Task.FromResult(System.Text.Encoding.UTF8.GetBytes(json));
        }
    }
}
