using System;
using System.Threading.Tasks;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Metadata;

namespace LightBDD.Extensions.ContextualAsyncExecution
{
    //TODO: test in separate project
    //TODO: rename project to be consistent
    public class ScenarioExecutionContextExtension : IScenarioExecutionExtension
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