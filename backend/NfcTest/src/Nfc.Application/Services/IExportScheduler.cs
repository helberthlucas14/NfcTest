using Nfc.Application.Export;

namespace Nfc.Application.Services
{
    public interface IExportScheduler
    {
        Task<string> SheculerExportAsync(ExportType type, CancellationToken cancellationToken);
    }


}
