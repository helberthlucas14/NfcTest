using System.Reflection.Metadata;

namespace Nfc.Application.Export
{
    public interface IExportService
    {
        ExportType Type { get; }

        Task<byte[]> ExportAsync<T>(T data, CancellationToken cancellationToken);
    }
}
