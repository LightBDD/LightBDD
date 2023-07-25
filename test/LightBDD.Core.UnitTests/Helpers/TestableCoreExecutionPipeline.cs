using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using LightBDD.Core.Configuration;
using LightBDD.Core.Discovery;
using LightBDD.Core.Execution;
using LightBDD.Core.Results;

namespace LightBDD.Core.UnitTests.Helpers
{
    internal class TestableCoreExecutionPipeline : ExecutionPipeline
    {
        public static readonly TestableCoreExecutionPipeline Default = Create();

        private TestableCoreExecutionPipeline(Assembly testAssembly, Action<LightBddConfiguration> onConfigure = null) : base(testAssembly, TestLightBddConfiguration.SetTestDefaults + onConfigure)
        {
        }

        public static TestableCoreExecutionPipeline Create(Action<LightBddConfiguration> onConfigure = null) => new(typeof(TestableCoreExecutionPipeline).Assembly, onConfigure);

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
