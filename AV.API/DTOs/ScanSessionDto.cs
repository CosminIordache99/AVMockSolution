using AV.Engine.Core.Entities;
using AV.Engine.Core.Enums;

namespace AV.API.DTOs
{
    public record ScanSessionDto(
    DateTime StartTimestamp,
    DateTime StopTimestamp,
    string Reason,
    IReadOnlyList<ThreatInfoDto> Threats
    );

    public record ThreatInfoDto(string FilePath, string ThreatName);


    public record RealTimeThreatDto(
    DateTime Timestamp,
    string FilePath,
    string ThreatName
);

    public record RealTimeStatusDto(
        DateTime Timestamp,
        bool IsEnabled
    );

    public record ScanResultDto(
        IReadOnlyList<ScanSessionDto> OnDemandSessions,
        IReadOnlyList<RealTimeThreatDto> RealTimeThreats,
        IReadOnlyList<RealTimeStatusDto> RealTimeStatuses
    );
    static class ScanSessionMapper
    {
        public static ScanResultDto ToScanResults(IEnumerable<ScanEvent> events)
        {
            var ordered = events.OrderBy(e => e.Timestamp).ToList();

            var sessions = new List<ScanSessionDto>();
            DateTime? start = null;
            string? reason = null;
            var threatsOnDemand = new List<ThreatInfoDto>();

            foreach (var e in ordered)
            {
                switch (e)
                {
                    case ScanStartedEvent se when se.ScanType == ScanType.OnDemand:
                        start = se.Timestamp;
                        threatsOnDemand.Clear();
                        reason = null;
                        break;

                    case ThreatDetectedEvent te
                         when te.ScanType == ScanType.OnDemand
                           && start.HasValue:
                        threatsOnDemand.Add(new ThreatInfoDto(
                            te.Threat.FilePath,
                            te.Threat.ThreatName
                        ));
                        break;

                    case ScanStoppedEvent se
                         when start.HasValue:
                        reason = se.Reason ?? string.Empty;
                        sessions.Add(new ScanSessionDto(
                            StartTimestamp: start.Value,
                            StopTimestamp: se.Timestamp,
                            Reason: reason,
                            Threats: threatsOnDemand.ToList()
                        ));
                        start = null;
                        threatsOnDemand.Clear();
                        break;
                }
            }

            var rtThreats = ordered
                .OfType<ThreatDetectedEvent>()
                .Where(te => te.ScanType == ScanType.RealTime)
                .Select(te => new RealTimeThreatDto(
                    te.Timestamp,
                    te.Threat.FilePath,
                    te.Threat.ThreatName
                ))
                .ToList();

            var rtStatuses = ordered
                .OfType<RealTimeScanStatusChangedEvent>()
                .Select(e => new RealTimeStatusDto(
                    e.Timestamp,
                    e.IsEnabled
                ))
                .ToList();

            return new ScanResultDto(
                OnDemandSessions: sessions,
                RealTimeThreats: rtThreats,
                RealTimeStatuses: rtStatuses
            );
        }
    }
}
