
using AV.Engine.Core.Entities;
using AV.Engine.Core.Enums;
using AV.Engine.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace AV.Engine
{
    public class AVEngine : IAVEngine
    {
        private readonly IEventStore _eventStore;
        private readonly IScanEngine _scanEngine;
        private readonly IThreatSimulator _threatSimulator;
        private readonly ILogger<AVEngine> _logger;

        private readonly SemaphoreSlim _realTimeSemaphore = new(1, 1);
        private readonly SemaphoreSlim _onDemandSemaphore = new(1, 1);

        private CancellationTokenSource _realTimeCts;
        private CancellationTokenSource _onDemandCts;
        private string _currentScanId;

        private volatile bool _isRealTimeEnabled;
        private volatile bool _isOnDemandScanRunning;
        private volatile bool _disposed;

        public AVEngine(
            IEventStore eventStore,
            IScanEngine scanEngine,
            IThreatSimulator threatSimulator,
            ILogger<AVEngine> logger)
        {
            _eventStore = eventStore ?? throw new ArgumentNullException(nameof(eventStore));
            _scanEngine = scanEngine ?? throw new ArgumentNullException(nameof(scanEngine));
            _threatSimulator = threatSimulator ?? throw new ArgumentNullException(nameof(threatSimulator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public bool IsRealTimeEnabled => _isRealTimeEnabled;
        public bool IsOnDemandScanRunning => _isOnDemandScanRunning;

        public event EventHandler<ThreatDetectedEvent> ThreatDetected;
        public event EventHandler<ScanStartedEvent> ScanStarted;
        public event EventHandler<ScanStoppedEvent> ScanStopped;
        public event EventHandler<RealTimeScanStatusChangedEvent> RealTimeScanStatusChanged;

        public async Task EnableRealTimeAsync()
        {
            ThrowIfDisposed();

            if (_isRealTimeEnabled)
            {
                _logger.LogInformation("Real-time scanning is already enabled");
                return;
            }

            await _realTimeSemaphore.WaitAsync();
            try
            {
                if (_isRealTimeEnabled) return;

                _realTimeCts = new CancellationTokenSource();
                _isRealTimeEnabled = true;

                var statusEvent = new RealTimeScanStatusChangedEvent { IsEnabled = true };
                await _eventStore.AddAsync(statusEvent);
                RealTimeScanStatusChanged?.Invoke(this, statusEvent);

                _ = Task.Run(async () => await RunRealTimeScanningAsync(_realTimeCts.Token));

                _logger.LogInformation("Real-time scanning enabled");
            }
            finally
            {
                _realTimeSemaphore.Release();
            }
        }

        public async Task DisableRealTimeAsync()
        {
            ThrowIfDisposed();

            if (!_isRealTimeEnabled)
            {
                _logger.LogInformation("Real-time scanning is already disabled");
                return;
            }

            await _realTimeSemaphore.WaitAsync();
            try
            {
                if (!_isRealTimeEnabled) return;

                _realTimeCts?.Cancel();
                _isRealTimeEnabled = false;

                var statusEvent = new RealTimeScanStatusChangedEvent { IsEnabled = false };
                await _eventStore.AddAsync(statusEvent);
                RealTimeScanStatusChanged?.Invoke(this, statusEvent);

                _logger.LogInformation("Real-time scanning disabled");
            }
            finally
            {
                _realTimeSemaphore.Release();
            }
        }

        public async Task<ScanResult> StartOnDemandScanAsync()
        {
            ThrowIfDisposed();

            if (_isOnDemandScanRunning)
            {
                _logger.LogWarning("On-demand scan is already running");
                return ScanResult.AlreadyRunning;
            }

            await _onDemandSemaphore.WaitAsync();
            try
            {
                if (_isOnDemandScanRunning)
                    return ScanResult.AlreadyRunning;

                _onDemandCts = new CancellationTokenSource();
                _currentScanId = Guid.NewGuid().ToString();
                _isOnDemandScanRunning = true;

                var startedEvent = new ScanStartedEvent { ScanId = _currentScanId, ScanType = ScanType.OnDemand };
                await _eventStore.AddAsync(startedEvent);
                ScanStarted?.Invoke(this, startedEvent);

                _ = Task.Run(async () => await RunOnDemandScanAsync(_onDemandCts.Token));

                _logger.LogInformation("On-demand scan started with ID: {ScanId}", _currentScanId);
                return ScanResult.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to start on-demand scan");
                return ScanResult.Failed;
            }
            finally
            {
                _onDemandSemaphore.Release();
            }
        }

        public async Task<bool> StopOnDemandScanAsync()
        {
            ThrowIfDisposed();

            if (!_isOnDemandScanRunning)
            {
                _logger.LogInformation("No on-demand scan is running");
                return false;
            }

            await _onDemandSemaphore.WaitAsync();
            try
            {
                if (!_isOnDemandScanRunning) return false;

                _onDemandCts?.Cancel();
                _logger.LogInformation("On-demand scan stop requested for scan ID: {ScanId}", _currentScanId);
                return true;
            }
            finally
            {
                _onDemandSemaphore.Release();
            }
        }

        public async Task<IReadOnlyList<ScanEvent>> GetPersistedEventsAsync()
        {
            ThrowIfDisposed();
            return await _eventStore.GetAllAsync();
        }

        public async Task ClearPersistedEventsAsync()
        {
            ThrowIfDisposed();
            await _eventStore.ClearAsync();
            _logger.LogInformation("Persisted events cleared");
        }

        private async Task RunRealTimeScanningAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);

                    if (_threatSimulator.ShouldDetectThreat())
                    {
                        var threat = _threatSimulator.GenerateRandomThreat(ScanType.RealTime);
                        var threatEvent = new ThreatDetectedEvent
                        {
                            Threat = threat,
                            ScanType = ScanType.RealTime
                        };

                        await _eventStore.AddAsync(threatEvent);
                        ThreatDetected?.Invoke(this, threatEvent);

                        _logger.LogWarning("Real-time threat detected: {Threat}", threat);
                    }
                }
            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation("Real-time scanning was cancelled");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in real-time scanning loop");
            }
        }

        private async Task RunOnDemandScanAsync(CancellationToken cancellationToken)
        {
            var scanId = _currentScanId;
            string stopReason = "Completed";

            try
            {
                var threats = await _scanEngine.ScanAsync(cancellationToken);

                foreach (var threat in threats)
                {
                    var threatEvent = new ThreatDetectedEvent
                    {
                        ScanId = _currentScanId,
                        Threat = threat,
                        ScanType = ScanType.OnDemand
                    };

                    await _eventStore.AddAsync(threatEvent);
                    ThreatDetected?.Invoke(this, threatEvent);

                    _logger.LogWarning("On-demand threat detected: {Threat}", threat);
                }

                _logger.LogInformation("On-demand scan completed. Found {ThreatCount} threats", threats.Count);
            }
            catch (TaskCanceledException)
            {
                stopReason = "Forced";
                _logger.LogInformation("On-demand scan was cancelled");
            }
            catch (Exception ex)
            {
                stopReason = "Error";
                _logger.LogError(ex, "Error during on-demand scan");
            }
            finally
            {
                await _onDemandSemaphore.WaitAsync();
                try
                {
                    _isOnDemandScanRunning = false;
                    _currentScanId = null;

                    var stoppedEvent = new ScanStoppedEvent
                    {
                        ScanId = scanId,
                        Reason = stopReason
                    };

                    await _eventStore.AddAsync(stoppedEvent);
                    ScanStopped?.Invoke(this, stoppedEvent);
                }
                finally
                {
                    _onDemandSemaphore.Release();
                }
            }
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(AVEngine));
        }

        public void Dispose()
        {
            if (_disposed) return;

            _realTimeCts?.Cancel();
            _onDemandCts?.Cancel();

            _realTimeSemaphore?.Dispose();
            _onDemandSemaphore?.Dispose();
            _realTimeCts?.Dispose();
            _onDemandCts?.Dispose();

            _disposed = true;
        }
    }
}
