using AV.Engine.Core.Entities;
using AV.Engine.Core.Enums;
using AV.Engine.Core.Interfaces;

namespace AV.Engine
{
    public sealed class ThreatSimulator : IThreatSimulator
    {
        private readonly Random _random;
        private static readonly string[] ThreatNames = {
            "Trojan.Generic", "Virus.Win32.Dummy", "Adware.MockAd",
            "Malware.Suspicious", "Rootkit.Stealth", "Worm.Network"
        };

        public ThreatSimulator()
        {
            _random = new Random();
        }

        public ThreatInfo GenerateRandomThreat(ScanType scanType)
        {
            var basePath = scanType == ScanType.RealTime
                ? @"C:\Windows\Temp\"
                : @"C:\Users\Documents\";

            var fileName = $"file_{_random.Next(1, 1000)}.{GetRandomExtension()}";
            var threatName = ThreatNames[_random.Next(ThreatNames.Length)];

            return new ThreatInfo($"{basePath}{fileName}", threatName);
        }

        public IReadOnlyList<ThreatInfo> GenerateRandomThreats(int count, ScanType scanType)
        {
            var threats = new List<ThreatInfo>();
            for (int i = 0; i < count; i++)
            {
                threats.Add(GenerateRandomThreat(scanType));
            }
            return threats.AsReadOnly();
        }

        public bool ShouldDetectThreat()
        {
            return _random.NextDouble() < 0.3; // 30% chance
        }

        public TimeSpan GetRandomScanDuration()
        {
            return TimeSpan.FromSeconds(_random.Next(10, 31));
        }

        private string GetRandomExtension()
        {
            var extensions = new[] { "exe", "dll", "bat", "scr", "com" };
            return extensions[_random.Next(extensions.Length)];
        }
    }
}
