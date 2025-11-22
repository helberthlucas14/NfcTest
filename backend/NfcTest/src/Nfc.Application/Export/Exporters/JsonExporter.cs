using Nfc.Application.Export.Interfaces;

namespace Nfc.Application.Export.Exporters
{
    public class JsonExporter : IExporter
    {
        public ExportType Type => ExportType.JSON;
        public Task<byte[]> ExportAsync<T>(IEnumerable<T> data, CancellationToken cancellationToken)
        {
            Thread.Sleep(TimeSpan.FromSeconds(5));
            var json = System.Text.Json.JsonSerializer.Serialize(data);
            Thread.Sleep(TimeSpan.FromSeconds(5));
            return Task.FromResult(System.Text.Encoding.UTF8.GetBytes(json));
        }
    }
}
