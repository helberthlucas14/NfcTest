using Nfc.Application.Export.Interfaces;

namespace Nfc.Application.Export.Exporters
{
    public class JsonExporter : IExporter
    {
        public ExportType Type => ExportType.JSON;
        public Task<byte[]> ExportAsync<T>(IEnumerable<T> data, CancellationToken cancellationToken)
        {
            var randomNumber = new Random().Next(1, 1000);
            Thread.Sleep(TimeSpan.FromSeconds(5));
            var json = System.Text.Json.JsonSerializer.Serialize(data);
            if (randomNumber % 2 > 0)
                throw new Exception("Falha ao exportar");
            return Task.FromResult(System.Text.Encoding.UTF8.GetBytes(json));
        }
    }
}
