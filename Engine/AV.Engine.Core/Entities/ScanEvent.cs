using AV.Engine.Core.Enums;

namespace AV.Engine.Core.Entities
{

    public abstract class ScanEvent
    {
        protected ScanEvent()
        {
            Timestamp = DateTime.UtcNow;
            Id = Guid.NewGuid();
        }

        public Guid Id { get; }
        public DateTime Timestamp { get; }
        public abstract string EventType { get; }
    }

    public sealed class ScanStartedEvent : ScanEvent
    {
        public override string EventType => "ScanStarted";
        public string ScanId { get; set; }
        public ScanType ScanType { get; set; }
    }

    public sealed class ScanStoppedEvent : ScanEvent
    {
        public override string EventType => "ScanStopped";
        public string ScanId { get; set; }
        public string Reason { get; set; }
        public ScanType ScanType { get; set; }
    }

    public sealed class ThreatDetectedEvent : ScanEvent
    {
        public override string EventType => "ThreatDetected";
        public ThreatInfo Threat { get; set; }
        public string ScanId { get; set; }
        public ScanType ScanType { get; set; }
    }

    public sealed class RealTimeScanStatusChangedEvent : ScanEvent
    {
        public override string EventType => "RealTimeScanStatusChanged";
        public bool IsEnabled { get; set; }
    }
}
