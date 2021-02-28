using System;
using LightBDD.Core.Execution;

namespace LightBDD.Core.ExecutionContext.Implementation
{
    internal class CurrentScenarioProperty : IContextProperty
    {
        public IScenario Scenario { get; set; }
        public object Fixture { get; set; }
    }
}