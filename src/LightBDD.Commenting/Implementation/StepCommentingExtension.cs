using System;
using System.Threading.Tasks;
using LightBDD.Core.Execution;
using LightBDD.Core.Extensibility.Execution;
using LightBDD.ExecutionContext;

namespace LightBDD.Commenting.Implementation
{
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