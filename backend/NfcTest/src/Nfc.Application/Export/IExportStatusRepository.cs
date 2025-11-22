namespace Nfc.Application.Export
{
    public interface IExportStatusRepository
    {
        Task SaveAsync(ExportStatus status, CancellationToken cancellationToken);
        Task<ExportStatus?> GetAsync(string jobId, CancellationToken cancellationToken);
    }
}