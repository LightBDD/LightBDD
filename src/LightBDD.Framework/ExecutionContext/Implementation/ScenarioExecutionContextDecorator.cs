using System;
using System.Diagnostics;
using System.Threading.Tasks;
using LightBDD.Core.Execution;
using LightBDD.Core.Extensibility.Execution;

namespace LightBDD.Framework.ExecutionContext.Implementation
{
    [DebuggerStepThrough]
    internal class ScenarioExecutionContextDecorator : IScenarioDecorator
    {
        public async Task ExecuteAsync(IScenario scenario, Func<Task> scenarioInvocation)
        {
            try
            {
                ScenarioExecutionContext.Current = new ScenarioExecutionContext();
                await scenarioInvocation();
            }
            finally
            {
                ScenarioExecutionContext.Current = null;
            }
        }
    }
}