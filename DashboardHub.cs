using System;
using System.Threading.Tasks;
using AutoMapper;
using Data.Service.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Telemetry.Service.Client;
using Telemetry.Service.Client.Models;
using MonitoringData = User.Service.Contracts.MonitoringData;
using PoolStatistic = User.Service.Contracts.PoolStatistic;

namespace User.Service.Hubs
{
    [Authorize]
    public class DashboardHub : HubBase<IDashboardHubClient>
    {
        private readonly IHubContext<DashboardHub> _hubContext;
        private readonly ILogger<DashboardHub> _logger;
        private readonly IMapper _mapper;
        private readonly ITelemetryDataService _telemetry;

        public DashboardHub(
            IMcryptoDataAPI mcryptoDataApi,
            IMapper mapper,
            ILogger<DashboardHub> logger,
            IHubContext<DashboardHub> hubContext,
            ITelemetryDataService telemetry)
        {
            if (mcryptoDataApi == null)
            {
                throw new ArgumentNullException(nameof(mcryptoDataApi));
            }

            if (mapper == null)
            {
                throw new ArgumentNullException(nameof(mapper));
            }

            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            if (telemetry == null)
            {
                throw new ArgumentNullException(nameof(telemetry));
            }

            _mapper = mapper;
            _logger = logger;
            _hubContext = hubContext;
            _telemetry = telemetry;
        }

        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, MinerName);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, MinerName);
        }

        public async Task RefreshMinerStatistics(DateTime beginDate, DateTime endDate, Int32 type, String[] algorithms)
        {
            try
            {
                var workersStatistics = new PoolStatistic();
                try
                {
                    var model = new GetMinerStatisticsModel
                    {
                        DateFrom = beginDate,
                        DateTo = endDate,
                        Miner = MinerName,
                        Algorithms = algorithms,
                        Interval = type
                    };
                    var workerStatisticsData =
                        await _telemetry.GetMinerStatisticsAsync(model);
                    workersStatistics = _mapper.Map<PoolStatistic>(workerStatisticsData);
                }
                finally
                {
                    await _hubContext.Clients.Client(Context.ConnectionId)
                        .SendAsync(nameof(IDashboardHubClient.MinerStatisticsBatch), workersStatistics);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(default, e, $"{nameof(RefreshMinerStatistics)} failed");
                throw;
            }
        }

        public async Task RefreshMinerStatisticsByTelemetry(DateTime beginDate, DateTime endDate, Int32 type,
            String[] algorithms)
        {
            try
            {
                var workersStatistics = new PoolStatistic();
                try
                {
                    var model = new GetMinerStatisticByTelemetryModel
                    {
                        Miner = MinerName,
                        Interval = type,
                        To = endDate,
                        FromProperty = beginDate,
                        Algorithms = algorithms
                    };
                    var workerStatisticsData =
                        await _telemetry.GetMinerStatisticsByTelemetryAsync(model);
                    workersStatistics = _mapper.Map<PoolStatistic>(workerStatisticsData);
                }
                finally
                {
                    await _hubContext.Clients.Client(Context.ConnectionId)
                        .SendAsync(nameof(IDashboardHubClient.MinerStatisticsByTelemetryBatch), workersStatistics);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(default, e, $"{nameof(RefreshMinerStatisticsByTelemetry)} failed");
                throw;
            }
        }

        public async Task RefreshMonitoring()
        {
            try
            {
                MonitoringData monitoring = null;

                try
                {
                    var monitoringData = await _telemetry.GetMonitoringAsync(MinerName);
                    monitoring = _mapper.Map<MonitoringData>(monitoringData);
                }
                catch
                {
                    monitoring = new MonitoringData();
                }
                finally
                {
                    await _hubContext.Clients.Client(Context.ConnectionId)
                        .SendAsync(nameof(IDashboardHubClient.MonitoringUpdate), monitoring);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(default, e, $"{nameof(RefreshMonitoring)} failed");
                throw;
            }
        }
    }
}