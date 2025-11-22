using Nfc.Application.Export;

namespace Nfc.Application.Export.Interfaces
{
    public interface IExportFileStorage
    {
        Task<StoredFileInfo> SaveAsync(string jobId, ExportType type, Stream content, CancellationToken cancellationToken);
        Task<StoredFileInfo?> OpenReadAsync(string jobId, ExportType type, CancellationToken cancellationToken);
        Task<string?> GetPublicUrlAsync(string jobId, ExportType type, TimeSpan? expires = null, CancellationToken cancellationToken = default);
    }
}