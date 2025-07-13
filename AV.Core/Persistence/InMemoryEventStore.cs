using AV.Core.Models;

namespace AV.Core.Persistence
{
    public class InMemoryEventStore
    {
        private readonly List<ScanEvent> _events = new();

        public void Add(ScanEvent e) => _events.Add(e);
        public IReadOnlyList<ScanEvent> GetAll() => _events.ToList();
        public void Clear() => _events.Clear();
    }
}
