namespace Nfc.Application.Export
{
    public interface IExportStatusNotifier
    {
        Task NotifyAsync(ExportStatus status, CancellationToken cancellationToken);
    }
}