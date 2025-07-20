namespace AV.Engine.Core.Entities
{

    public abstract class ScanEvent
    {
        public DateTime Timestamp { get; set; }
    }

    public class ScanStartedEvent : ScanEvent { }

    public class ScanStoppedEvent : ScanEvent
    {
        public string Reason { get; set; }
    }

    public class ThreatsFoundEvent : ScanEvent
    {
        public IReadOnlyList<ThreatInfo> Threats { get; set; }
    }
}
