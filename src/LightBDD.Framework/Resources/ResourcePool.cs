using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LightBDD.Framework.Resources
{
    public class ResourcePool<TResource> : IDisposable
    {
        private readonly Func<TResource> _resourceFactory;
        private readonly SemaphoreSlim _semaphore;
        private readonly ConcurrentQueue<TResource> _resources = new ConcurrentQueue<TResource>();
        private readonly ConcurrentQueue<TResource> _pool = new ConcurrentQueue<TResource>();
        private bool _controlDisposal;

        public ResourcePool(TResource[] resources, bool takeOwnership = true)
        {
            if (resources == null || !resources.Any())
                throw new ArgumentException("At least one resource has to be provided", nameof(resources));

            _semaphore = new SemaphoreSlim(resources.Length);
            _resourceFactory = () => throw new NotSupportedException("No new resources can be created in this mode - the ResourcePool has been initiated with pre-set list of resources");
            _controlDisposal = takeOwnership;

            foreach (var resource in resources)
            {
                _resources.Enqueue(resource);
                _pool.Enqueue(resource);
            }
        }

        public ResourcePool(Func<TResource> resourceFactory, int limit = int.MaxValue)
        {
            if (limit <= 0)
                throw new ArgumentException("Value has to be greater than 0", nameof(limit));
            _resourceFactory = resourceFactory ?? throw new ArgumentNullException(nameof(resourceFactory));
            _semaphore = new SemaphoreSlim(limit);
            _controlDisposal = true;
        }

        internal async Task<TResource> ObtainAsync(CancellationToken token)
        {
            await _semaphore.WaitAsync(token);
            if (_pool.TryDequeue(out var resource))
                return resource;
            return CreateNew();
        }

        private TResource CreateNew()
        {
            try
            {
                var resource = _resourceFactory();
                _resources.Enqueue(resource);
                return resource;
            }
            catch
            {
                _semaphore.Release();
                throw;
            }
        }

        internal void Release(TResource resource)
        {
            _pool.Enqueue(resource);
            _semaphore.Release();
        }

        public void Dispose()
        {
            if (!_controlDisposal)
                return;

            var exceptions = new List<Exception>();
            foreach (var resource in _resources)
            {
                try { (resource as IDisposable)?.Dispose(); }
                catch (Exception ex) { exceptions.Add(ex); }
            }
            if (exceptions.Any())
                throw new AggregateException("ResourcePool failed to dispose some resources", exceptions);
        }
    }

    public class ResourceProvider<TResource> : IDisposable
    {
        private readonly ResourcePool<TResource> _pool;
        private readonly object _lock = new object();
        private Task<TResource> _instanceTask;

        public ResourceProvider(ResourcePool<TResource> pool)
        {
            _pool = pool;
        }

        public Task<TResource> ObtainAsync() => ObtainAsync(CancellationToken.None);
        public Task<TResource> ObtainAsync(CancellationToken token)
        {
            if (_instanceTask == null)
            {
                lock (_lock)
                {
                    if (_instanceTask == null)
                        _instanceTask = _pool.ObtainAsync(token);
                }
            }

            return _instanceTask;
        }

        public void Dispose()
        {
            if (_instanceTask == null)
                return;

            try
            {
                _pool.Release(_instanceTask.GetAwaiter().GetResult());
                _instanceTask = null;
            }
            catch { }
        }
    }
}
