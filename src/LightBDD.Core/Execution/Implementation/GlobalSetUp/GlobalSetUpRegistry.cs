using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Dependencies;

namespace LightBDD.Core.Execution.Implementation.GlobalSetUp
{
    internal class GlobalSetUpRegistry
    {
        private readonly IGlobalSetUp[] _global;

        public GlobalSetUpRegistry(IEnumerable<IGlobalSetUp> globalSetUps)
        {
            _global = globalSetUps.ToArray();
        }

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

        public async Task TearDownAsync(IDependencyResolver resolver)
        {
            var exceptions = new List<Exception>();
            foreach (var runner in _global.AsEnumerable().Reverse())
            {
                try
                {
                    await runner.TearDownAsync(resolver);
                }
                catch (Exception e)
                {
                    exceptions.Add(e);
                }
            }

            if (exceptions.Any())
                throw new AggregateException($"Global clean up failed: {string.Join("\n", exceptions.Select(e => e.Message))}", exceptions);
        }
    }
}
