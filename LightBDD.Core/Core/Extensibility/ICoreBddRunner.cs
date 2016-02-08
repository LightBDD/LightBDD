using LightBDD.Core.Execution.Results;

namespace LightBDD.Core.Extensibility
{
    public interface ICoreBddRunner
    {
        IIntegrationContext IntegrationContext { get; }
        IFeatureResult GetFeatureResult();
        IScenarioRunner NewScenario();
    }
}