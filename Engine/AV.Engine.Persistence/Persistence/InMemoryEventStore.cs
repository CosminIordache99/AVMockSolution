using AV.Engine.Core.Entities;
using AV.Engine.Core.Interfaces;
using System.Collections.Concurrent;

namespace AV.Engine.Persistence.Persistence
{
    public sealed class InMemoryEventStore : IEventStore
    {
        private readonly ConcurrentQueue<ScanEvent> _events = new();
        private readonly SemaphoreSlim _semaphore = new(1, 1);

        public async Task AddAsync(ScanEvent scanEvent)
        {
            if (scanEvent == null)
                throw new ArgumentNullException(nameof(scanEvent));

            await _semaphore.WaitAsync();
            try
            {
                _events.Enqueue(scanEvent);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<IReadOnlyList<ScanEvent>> GetAllAsync()
        {
            await _semaphore.WaitAsync();
            try
            {
                return _events.ToList().AsReadOnly();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task ClearAsync()
        {
            await _semaphore.WaitAsync();
            try
            {
                _events.Clear();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<IReadOnlyList<ScanEvent>> GetEventsByTypeAsync<T>() where T : ScanEvent
        {
            await _semaphore.WaitAsync();
            try
            {
                return _events.OfType<T>().ToList().AsReadOnly();
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
