namespace AV.Engine.Core.Entities
{
    public class ThreatInfo
    {
        public int Id { get; set; }
        public string FilePath { get; set; }
        public string ThreatName { get; set; }
        public int ScanEventId { get; set; }
        public ScanEvent ScanEvent { get; set; }
    }
}
