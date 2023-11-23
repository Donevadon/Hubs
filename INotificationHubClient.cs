using System;
using Microsoft.AspNetCore.SignalR;

namespace User.Service.Hubs
{
    public interface INotificationHubClient : IClientProxy
    {
        void MessageSent(String message);
    }
}