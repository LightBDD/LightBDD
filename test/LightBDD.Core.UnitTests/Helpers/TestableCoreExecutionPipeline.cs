using System;
using System.Linq;
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
        public static readonly TestableCoreExecutionPipeline Default = new(typeof(TestableCoreExecutionPipeline).Assembly);
        private TestableCoreExecutionPipeline(Assembly testAssembly, Action<LightBddConfiguration> onConfigure = null) : base(testAssembly, onConfigure)
        {
        }

        public async Task<ITestRunResult> Execute(params Type[] features)
        {
            var disco = new ScenarioDiscoverer();
            var scenarioCases = features.SelectMany(f => disco.DiscoverFor(f.GetTypeInfo())).ToArray();
            return await Execute(scenarioCases);
        }
    }
}
