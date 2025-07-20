using AV.Engine.Core.Entities;

namespace AV.Engine.Core.Interfaces
{
    public interface IEventStore
    {
        Task AddAsync(ScanEvent scanEvent);
        Task<IEnumerable<ScanEvent>> GetAllAsync();
        Task ClearAsync();
    }
}
