using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LightBDD.Framework.Resources
{
    /// <summary>
    /// Class allowing to create a pool of resources.
    /// The class is designed for the scenarios where given resource can be used only by one task at a time, but can be reused later and it supports resource limits.
    /// The resource can be obtained from the pool with <see cref="ResourceHandle{TResource}"/> where it will return it back upon it's disposal.
    /// </summary>
    /// <typeparam name="TResource"></typeparam>
    public class ResourcePool<TResource> : IDisposable
    {
        private readonly Func<TResource> _resourceFactory;
        private readonly SemaphoreSlim _semaphore;
        private readonly ConcurrentQueue<TResource> _resources = new ConcurrentQueue<TResource>();
        private readonly ConcurrentQueue<TResource> _pool = new ConcurrentQueue<TResource>();
        private readonly bool _controlDisposal;

        /// <summary>
        /// Creates resource pool with pre-set list of resources, specified by <paramref name="resources"/> parameter.
        /// If <paramref name="takeOwnership"/> is set to true, the resources implementing <see cref="IDisposable"/> interface will be disposed upon pool disposal.
        /// </summary>
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

        /// <summary>
        /// Creates resource pool of dynamically managed resource number.
        /// The pool will start with no resources and grow up to the number of resources specified by <paramref name="limit"/> parameter.
        /// The new resources will be created if needed by calling <paramref name="resourceFactory"/> function.
        /// If resources implement <see cref="IDisposable"/> interface, they will be disposed upon pool disposal.
        /// </summary>
        public ResourcePool(Func<TResource> resourceFactory, int limit = int.MaxValue)
        {
            if (limit <= 0)
                throw new ArgumentException("Value has to be greater than 0", nameof(limit));
            _resourceFactory = resourceFactory ?? throw new ArgumentNullException(nameof(resourceFactory));
            _semaphore = new SemaphoreSlim(limit);
            _controlDisposal = true;
        }

        /// <summary>
        /// Creates a <see cref="ResourceHandle{TResource}"/> instance that allow to obtain the resource from the pool and return it upon it's disposal.
        /// </summary>
        public ResourceHandle<TResource> CreateHandle()
        {
            return new ResourceHandle<TResource>(this);
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

        /// <summary>
        /// Disposed the pool.
        /// If resources belonging to the pool are disposable and pool is configured to own them, they will be disposed as well.
        /// </summary>
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
}
