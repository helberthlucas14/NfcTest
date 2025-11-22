using Nfc.Application.Export;

namespace Nfc.Application.Services
{
    public record ExportStartData(long[] NoteIds, ExportType Format);

    public interface IExportNotasFiscalService
    {
        Task<ExportStartData> ValidateAndNormalizeAsync(long[] noteIds, ExportType format, CancellationToken cancellationToken);
        Task<byte[]> ExportAsync(ExportType type, IList<long> ids, CancellationToken cancellationToken);
    }
}
