using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace LightBDD.Core.Execution.Dependencies
{
    public class SimpleDependencyContainer : IDependencyContainer
    {
        private readonly ConcurrentQueue<IDisposable> _items = new ConcurrentQueue<IDisposable>();

        public async Task<object> ResolveAsync(Type type)
        {
            var item = Activator.CreateInstance(type);
            if (item is IDisposable disposable)
                _items.Enqueue(disposable);
            return item;
        }

        public async Task<object> RegisterInstance(object instance, bool takeOwnership)
        {
            if (takeOwnership && instance is IDisposable disposable)
                _items.Enqueue(disposable);
            return instance;
        }

        public void Dispose()
        {
            while (_items.TryDequeue(out var item))
            {
                try
                {
                    item.Dispose();
                }
                catch (Exception e)
                {
                    throw new InvalidOperationException($"Failed to dispose dependency '{item.GetType().Name}': {e.Message}", e);
                }
            }
        }

        public IDependencyContainer BeginScope()
        {
            return new SimpleDependencyContainer();
        }
    }
}