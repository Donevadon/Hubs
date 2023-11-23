using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Telemetry.Service.Client.Models;
using MonitoringData = User.Service.Contracts.MonitoringData;

namespace User.Service.Hubs
{
    public interface IDashboardHubClient
    {
        IClientProxy Proxy
        {
            set;
        }

        Task MinerStatisticsBatch(PoolStatistic statistics);
        Task MinerStatisticsByTelemetryBatch(PoolStatistic statistics);

        Task MonitoringUpdate(MonitoringData data);
        Task OnMessage(String message);
    }
}