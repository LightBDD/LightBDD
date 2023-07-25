#nullable enable
using LightBDD.Core.Execution;
using LightBDD.Core.Extensibility;

namespace LightBDD.Core.ExecutionContext.Implementation
{
    internal class CurrentScenarioProperty : IContextProperty
    {
        public IScenario? Scenario { get; set; }
        public ICoreScenarioStepsRunner? StepsRunner { get; set; }
    }
}