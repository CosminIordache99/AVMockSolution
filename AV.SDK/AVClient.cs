using AV.Engine.Core.Entities;
using AV.Engine.Core.Enums;
using AV.Engine.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace AV.SDK
{
    public class AVClient : IAsyncDisposable
    {
        private readonly IAVEngine _engine;
        private readonly ILogger<AVClient> _logger;
        private bool _disposed;

        public AVClient(IAVEngine engine, ILogger<AVClient> logger)
        {
            _engine = engine ?? throw new ArgumentNullException(nameof(engine));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public bool IsRealTimeEnabled => _engine.IsRealTimeEnabled;
        public bool IsOnDemandScanRunning => _engine.IsOnDemandScanRunning;

        /// <summary> Start a one-off scan. </summary>
        public Task<ScanResult> StartOnDemandScanAsync(CancellationToken ct = default)
            => _engine.StartOnDemandScanAsync();

        /// <summary> Request a stop; returns false if none was running. </summary>
        public Task<bool> StopOnDemandScanAsync(CancellationToken ct = default)
            => _engine.StopOnDemandScanAsync();

        public Task EnableRealTimeAsync(CancellationToken ct = default)
            => _engine.EnableRealTimeAsync();

        public Task DisableRealTimeAsync(CancellationToken ct = default)
            => _engine.DisableRealTimeAsync();

        public Task<IReadOnlyList<ScanEvent>> GetPersistedEventsAsync(CancellationToken ct = default)
            => _engine.GetPersistedEventsAsync();

        public Task ClearPersistedEventsAsync(CancellationToken ct = default)
            => _engine.ClearPersistedEventsAsync();

        public event EventHandler<ScanStartedEvent> ScanStarted
        {
            add => _engine.ScanStarted += value;
            remove => _engine.ScanStarted -= value;
        }

        public event EventHandler<ThreatDetectedEvent> ThreatDetected
        {
            add => _engine.ThreatDetected += value;
            remove => _engine.ThreatDetected -= value;
        }

        public event EventHandler<ScanStoppedEvent> ScanStopped
        {
            add => _engine.ScanStopped += value;
            remove => _engine.ScanStopped -= value;
        }

        public event EventHandler<RealTimeScanStatusChangedEvent> RealTimeStatusChanged
        {
            add => _engine.RealTimeScanStatusChanged += value;
            remove => _engine.RealTimeScanStatusChanged -= value;
        }

        public async ValueTask DisposeAsync()
        {
            if (_disposed) return;
            _disposed = true;

            try
            {
                _engine.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error disposing AVEngine");
            }
        }
    }
}
