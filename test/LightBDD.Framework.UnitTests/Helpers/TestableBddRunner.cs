using System;
using System.Threading.Tasks;
using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Results;
using LightBDD.Framework.Configuration;
using LightBDD.Framework.Extensibility;
using LightBDD.ScenarioHelpers;

namespace LightBDD.Framework.UnitTests.Helpers
{
    internal class TestableBddRunner
    {
        private readonly RunnableScenarioFactory _factory;
        public static readonly TestableBddRunner Default = new();

        public TestableBddRunner()
        {
            var cfg = new LightBddConfiguration().WithFrameworkDefaults();
            cfg.Services.ConfigureProgressNotifiers().Clear();

            _factory = new RunnableScenarioFactory(new EngineContext(cfg));
        }

        public Task<IScenarioResult> RunScenario(Func<IBddRunner, Task> runner) =>
            _factory.CreateFor(new TestResults.TestFeatureInfo() { FeatureType = typeof(object) })
                .WithScenarioEntryMethod((_, _) => runner(BddRunnerContext.GetCurrent()))
                .Build()
                .RunAsync();
    }
}
