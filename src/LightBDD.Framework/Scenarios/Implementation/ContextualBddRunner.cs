using LightBDD.Core.Dependencies;
using LightBDD.Core.Execution;
using LightBDD.Core.Extensibility;
using System;
using System.Threading.Tasks;

namespace LightBDD.Framework.Scenarios.Implementation
{
    internal class ContextualBddRunner<TContext> : IBddRunner<TContext>, IIntegratedScenarioBuilder<TContext>
    {
        public ContextualBddRunner(IBddRunner coreRunner, Func<object> contextProvider, bool takeOwnership)
        {
            Core = coreRunner.Integrate().Core.WithContext(Resolvable.Use(contextProvider, takeOwnership));
        }

        public ContextualBddRunner(IBddRunner coreRunner, Func<IDependencyResolver, object> contextResolver)
        {
            Core = coreRunner.Integrate().Core.WithContext(Resolvable.Use(contextResolver));
        }

        public IIntegratedScenarioBuilder<TContext> Integrate()
        {
            return this;
        }

        public async Task RunAsync()
        {
            try
            {
                await Core.RunAsync();
            }
            catch (ScenarioExecutionException e)
            {
                e.GetOriginal().Throw();
            }
        }

        public ICoreScenarioStepsRunner Core { get; }
    }
}