using AV.Engine.Core.Entities;
using AV.Engine.Core.Enums;

namespace AV.Engine.Core.Interfaces
{
    public interface IAVEngine : IDisposable
    {
        bool IsRealTimeEnabled { get; }
        bool IsOnDemandScanRunning { get; }

        /// <summary>
        /// Real-time scanning
        /// </summary>
        /// <returns></returns>
        Task EnableRealTimeAsync();
        Task DisableRealTimeAsync();

        /// <summary>
        /// On-demand scanning
        /// </summary>
        /// <returns></returns>
        Task<ScanResult> StartOnDemandScanAsync();
        Task<bool> StopOnDemandScanAsync();

        /// <summary>
        /// Events and persistence
        /// </summary>
        /// <returns></returns>
        Task<IReadOnlyList<ScanEvent>> GetPersistedEventsAsync();
        Task ClearPersistedEventsAsync();

        // Events
        event EventHandler<ThreatDetectedEvent> ThreatDetected;
        event EventHandler<ScanStartedEvent> ScanStarted;
        event EventHandler<ScanStoppedEvent> ScanStopped;
        event EventHandler<RealTimeScanStatusChangedEvent> RealTimeScanStatusChanged;
    }
}
