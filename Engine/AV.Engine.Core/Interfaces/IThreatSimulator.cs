using AV.Engine.Core.Entities;
using AV.Engine.Core.Enums;

namespace AV.Engine.Core.Interfaces
{
    public interface IThreatSimulator
    {
        ThreatInfo GenerateRandomThreat(ScanType scanType);
        IReadOnlyList<ThreatInfo> GenerateRandomThreats(int count, ScanType scanType);
        bool ShouldDetectThreat();
        TimeSpan GetRandomScanDuration();
    }
}
