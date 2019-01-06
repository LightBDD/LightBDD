using LightBDD.Core.Dependencies;
using LightBDD.Core.Execution;
using LightBDD.Core.Extensibility;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace LightBDD.Framework.Scenarios.Contextual.Implementation
{
    [DebuggerStepThrough]
    internal class ContextualBddRunner<TContext> : IBddRunner<TContext>, IIntegratedScenarioBuilder<TContext>
    {
        public ContextualBddRunner(IBddRunner coreRunner, Func<object> contextProvider, bool takeOwnership)
        {
            Core = coreRunner.Integrate().Core.WithContext(contextProvider, takeOwnership);
        }

        public ContextualBddRunner(IBddRunner coreRunner, Func<IDependencyResolver, object> contextResolver)
        {
            Core = coreRunner.Integrate().Core.WithContext(contextResolver);
        }

        public IIntegratedScenarioBuilder<TContext> Integrate()
        {
            return this;
        }

        public async Task RunAsync()
        {
            try
            {
                await Core
                    .WithCapturedScenarioDetailsIfNotSpecified()
                    .Build()
                    .ExecuteAsync();
            }
            catch (ScenarioExecutionException e)
            {
                e.GetOriginal().Throw();
            }
        }

        public ICoreScenarioBuilder Core { get; }
    }
}