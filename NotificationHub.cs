using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace User.Service.Hubs
{
    [Authorize]
    public class NotificationHub : HubBase<INotificationHubClient>
    {
        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, MinerName);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, Context.User.Identity.Name);
        }

        public void Send(String message)
        {
            Clients.All.MessageSent(message);
        }
    }
}