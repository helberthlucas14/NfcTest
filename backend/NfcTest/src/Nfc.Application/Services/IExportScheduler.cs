using Nfc.Application.Export;

namespace Nfc.Application.Services
{
    public interface IExportScheduler
    {
        Task<string> ScheduleExportAsync(ExportType type, long[] ids, CancellationToken cancellationToken);
    }


}
