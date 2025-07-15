namespace AV.Engine.Core.Entities
{

    public enum ScanEventType
    {
        ScanStarted,
        ScanStopped,
        ThreatsFound
    }
    public class ScanEvent
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public ScanEventType EventType { get; set; }
        public string? Reason { get; set; }
        public List<ThreatInfo> Threats { get; set; } = new();
    }
}
