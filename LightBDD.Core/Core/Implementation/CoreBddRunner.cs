using System;
using LightBDD.Core.Execution.Results;
using LightBDD.Core.Execution.Results.Implementation;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Extensibility.Implementation;

namespace LightBDD.Core.Implementation
{
    public abstract class CoreBddRunner : IBddRunner, ICoreBddRunner
    {
        public IIntegrationContext IntegrationContext { get; private set; }
        private readonly FeatureResult _featureResult;
        private readonly ScenarioExecutor _scenarioExecutor;

        protected CoreBddRunner(Type featureType, IIntegrationContext integrationContext)
        {
            IntegrationContext = integrationContext;
            _featureResult = new FeatureResult(IntegrationContext.MetadataProvider.GetFeatureInfo(featureType));

            _scenarioExecutor = new ScenarioExecutor();
            _scenarioExecutor.ScenarioExecuted += _featureResult.AddScenario;
        }

        public IFeatureResult GetFeatureResult()
        {
            return _featureResult;
        }

        public IScenarioRunner NewScenario()
        {
            return new ScenarioRunner(IntegrationContext.MetadataProvider, IntegrationContext.ExceptionToStatusMapper, _scenarioExecutor);
        }
    }
}