using System;
using System.Diagnostics;
using System.Threading.Tasks;
using LightBDD.Core.Execution;
using LightBDD.Core.Extensibility.Execution;
using LightBDD.Framework.ExecutionContext;

namespace LightBDD.Framework.Commenting.Implementation
{
    [DebuggerStepThrough]
    internal class CurrentStepDecorator : IStepDecorator
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