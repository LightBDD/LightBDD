using System;
using System.Threading.Tasks;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Extensibility.Execution;
using LightBDD.Core.Metadata;

namespace LightBDD.Extensions.ContextualAsyncExecution.Implementation
{
    //TODO: test in separate project
    //TODO: rename project to be consistent
    internal class ScenarioExecutionContextExtension : IScenarioExecutionExtension
    {
        public async Task ExecuteAsync(IScenarioInfo scenario, Func<Task> scenarioInvocation)
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