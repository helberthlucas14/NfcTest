using Microsoft.AspNetCore.SignalR;
using Nfc.Api.Hubs;
using Nfc.Application.Export;

namespace Nfc.Api.Notifications
{
    public class SignalRExportStatusNotifier : IExportStatusNotifier
    {
        private readonly IHubContext<ExportStatusHub> _hubContext;

        public SignalRExportStatusNotifier(IHubContext<ExportStatusHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifyAsync(ExportStatus status, CancellationToken cancellationToken)
        {
            await _hubContext.Clients.Group(status.JobId)
                .SendAsync("ExportStatusUpdated", status, cancellationToken);
        }
    }
}