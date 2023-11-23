using Microsoft.AspNetCore.SignalR;
using User.Service.Contracts;

namespace User.Service.Hubs
{
    public interface ITerminalHubClient : IClientProxy
    {
        void OnTerminalResponse(TerminalResponse message);
    }
}