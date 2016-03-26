using System;
using LightBDD.Core.Execution.Results;

namespace LightBDD.Core.Extensibility
{
    public interface ICoreBddRunner : IDisposable
    {
        IIntegrationContext IntegrationContext { get; }
        IFeatureResult GetFeatureResult();
        IScenarioRunner NewScenario();
        IBddRunner AsBddRunner();
    }
}