using Nfc.Application.Export;
using Nfc.Domain.Entity;

namespace Nfc.Application.Services
{
    public record ExportStartData(int[] NoteIds, string Format);
    public interface IExportNotasFiscalService
    {
        Task<ExportStartData> ValidateAndNormalizeAsync(long[] noteIds, ExportType format, CancellationToken cancellationToken);
    }
}
