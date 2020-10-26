using System;
using LightBDD.Core.Execution;

namespace LightBDD.Core.ExecutionContext.Implementation
{
    internal class CurrentScenarioProperty : IContextProperty
    {
        private IScenario? _scenario;

        public IScenario Scenario
        {
            get => _scenario ?? throw new InvalidOperationException("Current task is not executing any scenarios. Ensure that feature is used within task running scenario.");
            set => _scenario = value;
        }
    }
}