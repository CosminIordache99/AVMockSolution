using AV.Engine.Core.Entities;
using AV.Engine.Core.Interfaces;

namespace AV.SDK
{
    public class AVClient : IDisposable
    {
        private bool _disposed;

        private readonly IAVEngine _engine;
        public AVClient(IAVEngine engine) => _engine = engine;


        public bool RealTimeEnabled => _engine.RealTimeEnabled;

        public void EnableRealTime() => _engine.EnableRealTime();

        public void DisableRealTime() => _engine.DisableRealTime();

        public Task<StartScanResult> StartScanAsync() => _engine.StartScanAsync();

        public Task StopScanAsync() => _engine.StopScanAsync();

        public IEnumerable<ScanEvent> GetEvents() => _engine.GetPersistedEvents();

        public void ClearEvents() => _engine.ClearPersistedEvents();

        public event EventHandler<ThreatsFoundEvent> ThreatsFound
        {
            add => _engine.ThreatsFound += value;
            remove => _engine.ThreatsFound -= value;
        }
        public event EventHandler<ScanStartedEvent> ScanStarted
        {
            add => _engine.ScanStarted += value;
            remove => _engine.ScanStarted -= value;
        }
        public event EventHandler<ScanStoppedEvent> ScanStopped
        {
            add => _engine.ScanStopped += value;
            remove => _engine.ScanStopped -= value;
        }

        public void Dispose()
        {
            if (_disposed) return;
            if (_engine is IDisposable d) d.Dispose();
            _disposed = true;
        }
    }
}
