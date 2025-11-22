namespace Nfc.Application.Export.Storage
{
    public record StoredFileInfo(
        string ObjectKey,
        string ContentType,
        long Length,
        string? FileUrl
    );

    public interface IExportFileStorage
    {
        Task<StoredFileInfo> SaveAsync(
            string jobId,
            ExportType type,
            byte[] content,
            CancellationToken cancellationToken
        );

        Task<(Stream Stream, string ContentType)?> OpenReadAsync(
            string jobId,
            ExportType type,
            CancellationToken cancellationToken
        );
        Task<string?> GetPublicUrlAsync(
            string jobId,
            ExportType type,
            CancellationToken cancellationToken
        );
    }
}