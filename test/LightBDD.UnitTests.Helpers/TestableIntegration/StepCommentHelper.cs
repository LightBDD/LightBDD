using System;
using System.Threading;
using System.Threading.Tasks;
using LightBDD.Core.Execution;
using LightBDD.Core.Extensibility.Execution;

namespace LightBDD.UnitTests.Helpers.TestableIntegration
{
    public class StepCommentHelper : IStepExecutionExtension
    {
        public static readonly AsyncLocal<IStep> CurrentStep = new AsyncLocal<IStep>();

        public static void Comment(string commentReason)
        {
            CurrentStep.Value.Comment(commentReason);
        }

        public async Task ExecuteAsync(IStep step, Func<Task> stepInvocation)
        {
            CurrentStep.Value = step;
            try
            {
                await stepInvocation();
            }
            finally
            {
                CurrentStep.Value = null;
            }
        }
    }
}
