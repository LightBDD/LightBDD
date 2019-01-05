using LightBDD.Core.Configuration;
using LightBDD.Core.Execution;
using LightBDD.Core.Extensibility;
using LightBDD.Framework.Scenarios;
using System.Threading.Tasks;

namespace LightBDD.Framework.UnitTests.Scenarios.Extended.Helpers
{
    public class MockBddRunner<T> : IBddRunner<T>, IIntegratedScenarioBuilder<T>
    {
        public LightBddConfiguration Configuration { get; }

        public MockBddRunner(LightBddConfiguration configuration, ICoreScenarioBuilder scenarioRunner)
        {
            Configuration = configuration;
            Core = scenarioRunner;
        }

        public IIntegratedScenarioBuilder<T> Integrate() => this;

        public async Task RunAsync()
        {
            try
            {
                await Core.Build().Invoke();
            }
            catch (ScenarioExecutionException e)
            {
                e.GetOriginal().Throw();
            }
        }

        public ICoreScenarioBuilder Core { get; }
    }
}