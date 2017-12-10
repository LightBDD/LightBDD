using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LightBDD.Framework.Internals
{
    internal class AsyncConcurrentQueue<T> : IEnumerable<T>
    {
        private readonly ConcurrentQueue<T> _queue = new ConcurrentQueue<T>();
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(0);

        public void Enqueue(T value)
        {
            _queue.Enqueue(value);
            _semaphore.Release();
        }
        public async Task<T> DequeueAsync(CancellationToken cancellationToken = default)
        {
            await _semaphore.WaitAsync(cancellationToken);
            return _queue.TryDequeue(out var value)
                ? value
                : throw new InvalidOperationException($"Queue failed to provide value, which indicates a bug in {nameof(AsyncConcurrentQueue<T>)}.");
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _queue.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}