using System;
using System.Threading.Tasks;
using LightBDD.Core.Execution;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Extensibility.Execution;
using LightBDD.Extensions.ContextualAsyncExecution;

namespace LightBDD.Implementation
{
    //TODO: test in separate project
    //TODO: rename project to be consistent
    internal class StepCommentingExtension : IStepExecutionExtension
    {
        public async Task ExecuteAsync(IStep step, Func<Task> stepInvocation)
        {
            var stepProperty = ScenarioExecutionContext.Current.Get<CurrentStepProperty>();
            try
            {
                stepProperty.Step = step;
                await stepInvocation();
            }
            finally
            {
                stepProperty.Step = null;
            }
        }
    }
}