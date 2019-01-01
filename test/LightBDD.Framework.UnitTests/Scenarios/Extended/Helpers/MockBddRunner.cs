using System;
using System.Threading.Tasks;
using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;
using LightBDD.Framework.Scenarios;

namespace LightBDD.Framework.UnitTests.Scenarios.Extended.Helpers
{
    public class MockBddRunner<T> : IBddRunner<T>, IIntegratedScenarioBuilder<T>
    {

        public LightBddConfiguration Configuration { get; }

        public MockBddRunner(LightBddConfiguration configuration, ICoreScenarioBuilder scenarioRunner)
        {
            Configuration = configuration;
            Core= scenarioRunner;
        }

        public IIntegratedScenarioBuilder<T> Integrate() => this;

        public Task RunAsync()
        {
            return Build().Invoke();
        }

        public ICoreScenarioBuilder Core { get; }
        public IIntegratedScenarioBuilder<T> Configure(Action<ICoreScenarioBuilder> builder)
        {
            builder(Core);
            return this;
        }

        public Func<Task> Build()
        {
            return Core.RunScenarioAsync;
        }
    }
}