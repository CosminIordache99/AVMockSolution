using AV.Engine.Core.Entities;

namespace AV.Engine.Core.Interfaces
{
    public enum StartScanResult { Success, AlreadyRunning }
    public interface IAVEngine
    {
        bool RealTimeEnabled { get; }
        void EnableRealTime();
        void DisableRealTime();
        Task<StartScanResult> StartScanAsync();
        Task StopScanAsync();
        IEnumerable<ScanEvent> GetPersistedEvents();
        void ClearPersistedEvents();
        event EventHandler<ThreatsFoundEvent> ThreatsFound;
        event EventHandler<ScanStartedEvent> ScanStarted;
        event EventHandler<ScanStoppedEvent> ScanStopped;
    }
}
