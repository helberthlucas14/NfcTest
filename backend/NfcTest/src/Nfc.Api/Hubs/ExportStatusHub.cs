using Microsoft.AspNetCore.SignalR;

namespace Nfc.Api.Hubs
{
    public class ExportStatusHub : Hub
    {
        public async Task JoinJobGroup(string jobId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, jobId);
        }

        public async Task LeaveJobGroup(string jobId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, jobId);
        }
    }
}