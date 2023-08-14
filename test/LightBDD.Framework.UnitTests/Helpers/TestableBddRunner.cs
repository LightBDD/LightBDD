using System;
using System.Threading.Tasks;
using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Results;
using LightBDD.Framework.Configuration;
using LightBDD.Framework.Extensibility;
using LightBDD.UnitTests.Helpers;

namespace LightBDD.Framework.UnitTests.Helpers
{
    internal class TestableBddRunner
    {
        private readonly RunnableScenarioFactory _factory;
        public static TestableBddRunner Default = new();

        public TestableBddRunner()
        {
            var cfg = new LightBddConfiguration().WithFrameworkDefaults();
            cfg.RegisterProgressNotifiers().Clear();

            _factory = new RunnableScenarioFactory(new EngineContext(cfg));
        }

        public Task<IScenarioResult> RunScenario(Func<IBddRunner, Task> runner) =>
            _factory.CreateFor(new TestResults.TestFeatureInfo() { FeatureType = typeof(object) })
                .WithScenarioEntryMethod((_, _) => runner(BddRunnerContext.GetCurrent()))
                .Build()
                .RunAsync();
    }
}
