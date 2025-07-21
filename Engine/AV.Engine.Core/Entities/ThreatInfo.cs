namespace AV.Engine.Core.Entities
{
    public sealed class ThreatInfo : IEquatable<ThreatInfo>
    {
        public ThreatInfo(string filePath, string threatName)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path cannot be null or empty", nameof(filePath));
            if (string.IsNullOrWhiteSpace(threatName))
                throw new ArgumentException("Threat name cannot be null or empty", nameof(threatName));

            FilePath = filePath.Trim();
            ThreatName = threatName.Trim();
        }

        public string FilePath { get; }
        public string ThreatName { get; }

        public bool Equals(ThreatInfo other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return FilePath == other.FilePath && ThreatName == other.ThreatName;
        }

        public override bool Equals(object obj)
            => ReferenceEquals(this, obj) || obj is ThreatInfo other && Equals(other);

        public override int GetHashCode()
            => HashCode.Combine(FilePath, ThreatName);

        public override string ToString()
            => $"{ThreatName} at {FilePath}";
    }
}
