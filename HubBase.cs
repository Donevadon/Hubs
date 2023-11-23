using System;
using Microsoft.AspNetCore.SignalR;

namespace User.Service.Hubs
{
    public class HubBase<T> : Hub<T>
        where T : class
    {
        protected String MinerName => Context.User.Identity.Name;
    }
}