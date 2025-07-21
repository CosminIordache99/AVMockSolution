using AV.Engine.Core.Entities;
using AV.Engine.Core.Enums;
using AV.Engine.Core.Interfaces;

namespace AV.Engine
{
    public sealed class MockScanEngine : IScanEngine
    {
        private readonly IThreatSimulator _threatSimulator;
        private readonly Random _random;

        public MockScanEngine(IThreatSimulator threatSimulator)
        {
            _threatSimulator = threatSimulator ?? throw new ArgumentNullException(nameof(threatSimulator));
            _random = new Random();
        }

        public async Task<IReadOnlyList<ThreatInfo>> ScanAsync(CancellationToken cancellationToken)
        {
            var duration = _threatSimulator.GetRandomScanDuration();
            await Task.Delay(duration, cancellationToken);

            var threatCount = _random.Next(0, 4); // 0-3 threats
            return _threatSimulator.GenerateRandomThreats(threatCount, ScanType.OnDemand);
        }

        public async Task<ThreatInfo> SimulateRealTimeDetectionAsync()
        {
            await Task.Delay(TimeSpan.FromSeconds(_random.Next(1, 5)));
            return _threatSimulator.GenerateRandomThreat(ScanType.RealTime);
        }
    }
}
