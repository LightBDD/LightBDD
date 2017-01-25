using System;
using System.Threading.Tasks;
using LightBDD.Core.Metadata;

namespace LightBDD.Core.Extensibility.Execution
{
    public interface IScenarioExecutionExtension
    {
        Task ExecuteAsync(IScenarioInfo scenario, Func<Task> scenarioInvocation);
    }
}