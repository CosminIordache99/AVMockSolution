using AV.Engine.Core.Entities;

namespace AV.API.DTOs
{
    public record ScanSessionDto(
    DateTime StartTimestamp,
    DateTime StopTimestamp,
    string Reason,
    IReadOnlyList<ThreatInfoDto> Threats
    );

    public record ThreatInfoDto(string FilePath, string ThreatName);

    // A small mapper to keep controller clean:
    static class ScanSessionMapper
    {
        public static IReadOnlyList<ScanSessionDto> ToSessions(
            IEnumerable<ScanEvent> events)
        {
            var sessions = new List<ScanSessionDto>();
            DateTime? start = null;
            var threats = new List<ThreatInfoDto>();

            foreach (var e in events)
            {
                switch (e)
                {
                    case ScanStartedEvent se:
                        start = se.Timestamp;
                        threats.Clear();
                        break;
                    //case ThreatsFoundEvent te when start.HasValue:
                    //    threats.AddRange(te.Threats.Select(
                    //      t => new ThreatInfoDto(t.FilePath, t.ThreatName)));
                    //    break;
                    case ScanStoppedEvent se when start.HasValue:
                        sessions.Add(new ScanSessionDto(
                            start.Value, se.Timestamp, se.Reason, threats.ToList()));
                        start = null;
                        break;
                }
            }
            return sessions;
        }
    }
}
