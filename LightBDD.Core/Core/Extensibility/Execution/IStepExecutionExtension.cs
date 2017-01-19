using System;
using System.Threading.Tasks;
using LightBDD.Core.Execution;

namespace LightBDD.Core.Extensibility.Execution
{
    public interface IStepExecutionExtension
    {
        Task ExecuteAsync(IStep step, Func<Task> stepInvocation);
    }
}