using System.Collections.Generic;
using System.Threading.Tasks;
using User.Service.Model.Telemetry;

namespace User.Service.Hubs
{
    public interface IGpuTelemetryHubClient
    {
        Task SendTelemetry<T>(T message)
            where T : IEnumerable<RigTelemetry>;
    }
}