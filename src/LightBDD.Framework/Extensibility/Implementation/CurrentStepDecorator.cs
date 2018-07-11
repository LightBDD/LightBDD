using System;
using System.Diagnostics;
using System.Threading.Tasks;
using LightBDD.Core.Execution;
using LightBDD.Core.Extensibility.Execution;
using LightBDD.Framework.ExecutionContext;

namespace LightBDD.Framework.Extensibility.Implementation
{
    [DebuggerStepThrough]
    internal class CurrentStepDecorator : IStepDecorator
    {
        public async Task ExecuteAsync(IStep step, Func<Task> stepInvocation)
        {
            var stepProperty = ScenarioExecutionContext.Current.Get<CurrentStepProperty>();
            var last = stepProperty.Update(step);
            try
            {
                await stepInvocation();
            }
            finally
            {
                stepProperty.Update(last);
            }
        }
    }
}