using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Dependencies;

namespace LightBDD.Core.Execution.Implementation
{
    internal class GlobalInitializer
    {
        private readonly List<Func<IDependencyResolver, Task>> _setUps = new();
        private readonly Stack<Func<IDependencyResolver, Task>> _cleanUps = new();

        public async Task SetUpAsync(IDependencyResolver resolver)
        {
            try
            {
                foreach (var setUp in _setUps)
                    await setUp(resolver);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Global set up failed: {ex.Message}", ex);
            }
        }

        public async Task CleanUpAsync(IDependencyResolver resolver)
        {
            var exceptions = new List<Exception>();
            foreach (var cleanUp in _cleanUps)
            {
                try
                {
                    await cleanUp(resolver);
                }
                catch (Exception e)
                {
                    exceptions.Add(e);
                }
            }

            if (exceptions.Any())
                throw new AggregateException($"Global clean up failed: {string.Join("\n", exceptions.Select(e => e.Message))}", exceptions);
        }

        void RegisterSetUp(Func<IDependencyResolver, Task> setUp) => _setUps.Add(setUp);
        void RegisterCleanUp(Func<IDependencyResolver, Task> cleanUp) => _cleanUps.Push(cleanUp);

        public void RegisterSetUp<TDependency>(Func<TDependency, Task> setUp)
        {
            Task ExecuteSetUp(IDependencyResolver resolver) => setUp(resolver.Resolve<TDependency>());
            RegisterSetUp(ExecuteSetUp);
        }

        public void RegisterCleanUp<TDependency>(Func<TDependency, Task> cleanUp)
        {
            Task ExecuteCleanUp(IDependencyResolver resolver) => cleanUp(resolver.Resolve<TDependency>());
            RegisterCleanUp(ExecuteCleanUp);
        }

        public void RegisterSetUp(Func<Task> setUp)
        {
            Task ExecuteSetUp(IDependencyResolver _) => setUp();
            RegisterSetUp(ExecuteSetUp);
        }

        public void RegisterCleanUp(Func<Task> cleanUp)
        {
            Task ExecuteCleanUp(IDependencyResolver _) => cleanUp();
            RegisterCleanUp(ExecuteCleanUp);
        }
    }
}
