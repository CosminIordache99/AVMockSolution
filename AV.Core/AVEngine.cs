using AV.Core.Models;
using AV.Core.Persistence;

namespace AV.Core
{
    public class AVEngine : IAVEngine
    {
        private readonly InMemoryEventStore _store = new();
        private CancellationTokenSource _onDemandCts;
        private CancellationTokenSource _realTimeCts;
        private readonly object _lock = new();
        private readonly Random _rnd = new();

        //de mutat IAVEngine intr un proiect CORE astefel incat SDK sa aiba nevoie doar de ENGINE si nu si de CORE

        public bool RealTimeEnabled { get; private set; }

        public event EventHandler<ThreatsFoundEvent> ThreatsFound;
        public event EventHandler<ScanStartedEvent> ScanStarted;
        public event EventHandler<ScanStoppedEvent> ScanStopped;

        public void EnableRealTime()
        {
            if (RealTimeEnabled) return;
            RealTimeEnabled = true;
            _realTimeCts = new CancellationTokenSource();
            _ = RunRealTimeLoop(_realTimeCts.Token);
        }

        public void DisableRealTime()
        {
            if (!RealTimeEnabled) return;
            RealTimeEnabled = false;
            _realTimeCts.Cancel();
        }

        public async Task<StartScanResult> StartScanAsync()
        {
            lock (_lock)
            {
                if (_onDemandCts != null) return StartScanResult.AlreadyRunning;
                _onDemandCts = new CancellationTokenSource();
            }

            var startedEvent = new ScanStartedEvent { Timestamp = DateTime.UtcNow };
            _store.Add(startedEvent);
            ScanStarted?.Invoke(this, startedEvent);

            _ = RunOnDemandScan(_onDemandCts.Token);
            return StartScanResult.Success;
        }

        public Task StopScanAsync()
        {
            lock (_lock)
            {
                _onDemandCts?.Cancel();
            }
            return Task.CompletedTask;
        }

        public IEnumerable<ScanEvent> GetPersistedEvents() => _store.GetAll();
        public void ClearPersistedEvents() => _store.Clear();

        private async Task RunOnDemandScan(CancellationToken token)
        {
            var duration = TimeSpan.FromSeconds(_rnd.Next(10, 31));
            try
            {
                await Task.Delay(duration, token);
                var stopped = new ScanStoppedEvent
                {
                    Timestamp = DateTime.UtcNow,
                    Reason = "Completed"
                };
                _store.Add(stopped);
                ScanStopped?.Invoke(this, stopped);
            }
            catch (TaskCanceledException)
            {
                var stopped = new ScanStoppedEvent
                {
                    Timestamp = DateTime.UtcNow,
                    Reason = "Forced"
                };
                _store.Add(stopped);
                ScanStopped?.Invoke(this, stopped);
            }
            finally
            {
                var threats = new List<ThreatInfo>();
                int count = _rnd.Next(0, 4);
                for (int i = 0; i < count; i++)
                {
                    threats.Add(new ThreatInfo
                    {
                        FilePath = $@"C:\mock\file{i}.exe",
                        ThreatName = $"MockThreat{i}"
                    });
                }

                if (threats.Count > 0)
                {
                    var threatEvent = new ThreatsFoundEvent
                    {
                        Timestamp = DateTime.UtcNow,
                        Threats = threats
                    };
                    _store.Add(threatEvent);
                    ThreatsFound?.Invoke(this, threatEvent);
                }

                lock (_lock) { _onDemandCts = null; }
            }
        }

        private async Task RunRealTimeLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(5), token);
                if (token.IsCancellationRequested) break;

                if (_rnd.NextDouble() < 0.3)
                {
                    var threat = new ThreatInfo
                    {
                        FilePath = $@"C:\mock\realtime\file{_rnd.Next(1, 100)}.dll",
                        ThreatName = $"RTThreat{_rnd.Next(1, 10)}"
                    };
                    var evt = new ThreatsFoundEvent
                    {
                        Timestamp = DateTime.UtcNow,
                        Threats = new List<ThreatInfo> { threat }
                    };
                    _store.Add(evt);
                    ThreatsFound?.Invoke(this, evt);
                }
            }
        }
    }
}
