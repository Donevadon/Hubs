using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace User.Service.Hubs
{
    [Authorize]
    public class TelemetryHub : HubBase<IGpuTelemetryHubClient>
    {
        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, MinerName);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, MinerName);
        }
    }
}