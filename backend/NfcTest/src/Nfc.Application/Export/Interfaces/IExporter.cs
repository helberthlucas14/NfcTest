namespace Nfc.Application.Export.Interfaces
{
    public interface IExporter
    {
        ExportType Type { get; }
        Task<byte[]> ExportAsync<T>(IEnumerable<T> data, CancellationToken cancellationToken);
    }
}
