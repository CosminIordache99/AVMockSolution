using AV.Engine.Core.Entities;

namespace AV.Engine.Core.Interfaces
{
    public interface IScanEngine
    {
        Task<IReadOnlyList<ThreatInfo>> ScanAsync(CancellationToken cancellationToken);
        Task<ThreatInfo> SimulateRealTimeDetectionAsync();
    }
}
