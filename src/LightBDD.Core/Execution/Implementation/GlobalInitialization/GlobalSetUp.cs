using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Dependencies;

namespace LightBDD.Core.Execution.Implementation.GlobalInitialization
{
    internal class GlobalSetUp
    {
        private readonly List<IGlobalSetUp> _global = new();

        public async Task SetUpAsync(IDependencyResolver resolver)
        {
            try
            {
                foreach (var runner in _global)
                    await runner.SetUpAsync(resolver);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Global set up failed: {ex.Message}", ex);
            }
        }

        public async Task CleanUpAsync(IDependencyResolver resolver)
        {
            var exceptions = new List<Exception>();
            foreach (var runner in _global.AsEnumerable().Reverse())
            {
                try
                {
                    await runner.CleanUpAsync(resolver);
                }
                catch (Exception e)
                {
                    exceptions.Add(e);
                }
            }

            if (exceptions.Any())
                throw new AggregateException($"Global clean up failed: {string.Join("\n", exceptions.Select(e => e.Message))}", exceptions);
        }

        public void RegisterResource<TDependency>() where TDependency : IGlobalResourceSetUp
        {
            _global.Add(new GlobalResourceSetUp<TDependency>());
        }

        public void RegisterActivity(string name, Func<Task> setUp, Func<Task> cleanUp)
        {
            _global.Add(new GlobalActivitySetUp(name, setUp, cleanUp));
        }
    }
}
