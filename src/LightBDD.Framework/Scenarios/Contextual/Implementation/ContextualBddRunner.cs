using LightBDD.Core.Dependencies;
using LightBDD.Core.Extensibility;
using LightBDD.Framework.Extensibility;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using LightBDD.Core.Configuration;

namespace LightBDD.Framework.Scenarios.Contextual.Implementation
{
    [DebuggerStepThrough]
    internal class ContextualBddRunner<TContext> : IBddRunner<TContext>,IIntegratedScenarioBuilder<TContext>
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

        //TODO: remove?
        public Task RunAsync() => Build().Invoke();

        public ICoreScenarioBuilder Core { get; }

        public IIntegratedScenarioBuilder<TContext> Configure(Action<ICoreScenarioBuilder> builder)
        {
            builder(Core);
            return this;
        }
        //TODO: remove?
        public Func<Task> Build() => Core.RunScenarioAsync;
    }
}