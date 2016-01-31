using System;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Execution.Results;
using LightBDD.Core.Execution.Results.Implementation;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Extensibility.Implementation;
using LightBDD.Core.Metadata;

namespace LightBDD.Core.Implementation
{
    public abstract class CoreBddRunner : IBddRunner, ICoreBddRunner
    {
        public IIntegrationContext IntegrationContext { get; private set; }
        private FeatureResult _featureResult;

        protected CoreBddRunner(Type featureType, IIntegrationContext integrationContext)
        {
            IntegrationContext = integrationContext;
            _featureResult = new FeatureResult(IntegrationContext.MetadataProvider.GetFeatureInfo(featureType));
        }

        public IFeatureResult GetFeatureResult()
        {
            return _featureResult;
        }

        public IScenarioBuilder NewScenario()
        {
            return new ScenarioBuilder(IntegrationContext.MetadataProvider);
        }

        public async Task RunScenarioAsync(IScenarioInfo scenario)
        {
            var steps = scenario.Steps.Select((s, idx) => new RunnableStep(s, idx + 1, IntegrationContext.ExceptionToStatusMapper)).ToArray();
            try
            {
                foreach (var step in steps)
                    await step.Invoke(null);
            }
            finally
            {
                _featureResult.AddScenario(new ScenarioResult(scenario, steps.Select(s => s.Result).ToArray()));
            }
        }
    }
}