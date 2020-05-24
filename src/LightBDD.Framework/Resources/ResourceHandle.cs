using System;
using System.Threading;
using System.Threading.Tasks;

namespace LightBDD.Framework.Resources
{
    /// <summary>
    /// Class allowing to obtain resource from the <see cref="ResourcePool{TResource}"/> and return it back upon it's disposal.
    /// </summary>
    /// <typeparam name="TResource"></typeparam>
    public class ResourceHandle<TResource> : IDisposable
    {
        private readonly CancellationTokenSource _cancellationSource = new CancellationTokenSource();
        private readonly ResourcePool<TResource> _pool;
        private readonly object _lock = new object();
        private Task<TResource> _instanceTask;
        private bool _disposed;

        /// <summary>
        /// Constructor creating handle for the <see cref="ResourcePool{TResource}"/> specified by <paramref name="pool"/> parameter.
        /// </summary>
        /// <param name="pool"></param>
        public ResourceHandle(ResourcePool<TResource> pool)
        {
            _pool = pool;
        }

        /// <summary>
        /// Obtains the resource from the pool.<br/>
        /// If there is no available resources on the pool, the method will wait until one becomes available.<br/>
        /// If resource was already obtained, it will be returned immediately.
        /// </summary>
        /// <returns>Obtained resource.</returns>
        public Task<TResource> ObtainAsync() => ObtainAsync(CancellationToken.None);

        /// <summary>
        /// Obtains the resource from the pool.<br/>
        /// If there is no available resources on the pool, the method will wait until one becomes available.<br/>
        /// If resource was already obtained, it will be returned immediately.
        /// </summary>
        /// <returns>Obtained resource.</returns>
        /// <param name="token">Cancellation token.</param>
        public Task<TResource> ObtainAsync(CancellationToken token)
        {
            var instanceTask = _instanceTask ?? GetObtainTask();

            if (instanceTask.IsCompleted)
                return instanceTask;

            return instanceTask.ContinueWith(t => t.GetAwaiter().GetResult(), token);
        }

        private Task<TResource> GetObtainTask()
        {
            lock (_lock)
            {
                if (_disposed)
                    throw new ObjectDisposedException("ResourceHandle is already disposed");

                return _instanceTask ??= _pool.ObtainAsync(_cancellationSource.Token);
            }
        }

        /// <summary>
        /// Disposes handle and returns the resource back to the pool if it was obtained.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            lock (_lock)
            {
                _disposed = true;
                if (_instanceTask == null)
                    return;
                var instanceTask = _instanceTask;
                try
                {
                    _instanceTask = null;

                    if (!instanceTask.IsCompleted)
                        _cancellationSource.Cancel();

                    _pool.Release(instanceTask.GetAwaiter().GetResult());
                    _cancellationSource.Dispose();
                }
                catch
                {
                }
            }
        }
    }
}