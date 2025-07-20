using AV.Engine.Core.Entities;

namespace AV.Engine.Core.Interfaces
{
    public interface IEventStore
    {
        Task AddAsync(ScanEvent scanEvent);
        Task<IReadOnlyList<ScanEvent>> GetAllAsync();
        Task ClearAsync();
        Task<IReadOnlyList<ScanEvent>> GetEventsByTypeAsync<T>() where T : ScanEvent;
    }
}
