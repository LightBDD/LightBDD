using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using LightBDD.Core.Configuration;
using LightBDD.Core.Discovery;
using LightBDD.Core.Execution;
using LightBDD.Core.Results;
using LightBDD.Framework.Configuration;
using LightBDD.ScenarioHelpers;

namespace LightBDD.Framework.UnitTests.Helpers
{
    internal class TestableExecutionPipeline : ExecutionPipeline
    {
        public static readonly TestableExecutionPipeline Default = Create();

        private TestableExecutionPipeline(Assembly testAssembly, Action<LightBddConfiguration> onConfigure = null) : base(testAssembly, SetDefaults + onConfigure)
        {
        }

        private static void SetDefaults(LightBddConfiguration cfg)
        {
            cfg.WithFrameworkDefaults();
            cfg.RegisterReportGenerators().Clear();
            cfg.RegisterProgressNotifiers().Clear();
        }

        public static TestableExecutionPipeline Create(Action<LightBddConfiguration> onConfigure = null) => new(typeof(TestableExecutionPipeline).Assembly, onConfigure);

        public async Task<ITestRunResult> Execute(params Type[] features)
        {
            var disco = new ScenarioDiscoverer();
            var scenarioCases = features.SelectMany(f => disco.DiscoverFor(f.GetTypeInfo())).ToArray();
            return await Execute(scenarioCases);
        }

        public async Task<IFeatureResult> ExecuteFeature(Type feature)
        {
            var result = await Execute(feature);
            return result.Features.Single();
        }

        public async Task<IScenarioResult> ExecuteScenario<TFeature>(Expression<Func<TFeature, Task>> scenarioSelector)
        {
            var scenarios = new[] { ScenarioCase.CreateParameterless(typeof(TFeature).GetTypeInfo(), ParameterInfoHelper.GetMethodInfo(scenarioSelector)) };
            var result = await Execute(scenarios);
            return result.Features.Single().GetScenarios().Single();
        }
    }
}
