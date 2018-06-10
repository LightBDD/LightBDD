using System;
using System.Diagnostics;
using System.Threading.Tasks;
using LightBDD.Core.Execution;
using LightBDD.Core.Extensibility.Execution;
using LightBDD.Framework.ExecutionContext;

namespace LightBDD.Framework.Extensibility.Implementation
{
    [DebuggerStepThrough]
    internal class CurrentScenarioDecorator : IScenarioDecorator
    {
        public async Task ExecuteAsync(IScenario scenario, Func<Task> scenarioInvocation)
        {
            var property = ScenarioExecutionContext.Current.Get<CurrentScenarioProperty>();
            try
            {
                property.Scenario = scenario;
                await scenarioInvocation();
            }
            finally
            {
                property.Scenario = null;
            }
        }
    }
}