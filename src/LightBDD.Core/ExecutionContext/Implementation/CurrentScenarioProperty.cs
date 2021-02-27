using System;
using LightBDD.Core.Execution;

namespace LightBDD.Core.ExecutionContext.Implementation
{
    internal class CurrentScenarioProperty : IContextProperty
    {
        private IScenario _scenario;
        private object _fixture;

        public IScenario Scenario
        {
            get => _scenario ?? throw new InvalidOperationException("The current task does not run any initialized scenario. Ensure that feature is used within task running fully initialized scenario.");
            set => _scenario = value;
        }

        public object Fixture
        {
            get => _fixture ?? throw new InvalidOperationException("The current task does not run any scenario with available fixture object.");
            set => _fixture = value;
        }
    }
}