namespace Nfc.Application.Export
{
    public interface IExportStatusRepository
    {
        Task SaveAsync(ExportStatus status, CancellationToken cancellationToken);
        Task<ExportStatus?> GetAsync(Guid jobId, CancellationToken cancellationToken);
    }
}