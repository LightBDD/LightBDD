using System;
using System.Threading.Tasks;
using LightBDD.Core.Execution;
using LightBDD.Core.Execution.Implementation;
using LightBDD.Core.Metadata;

namespace LightBDD.Core.Extensibility
{
    //TODO: add tests
    public interface IExtendableExecutor
    {
        Task ExecuteScenario(IScenarioInfo scenario, Func<Task> scenarioInvocation);
        Task ExecuteStep(IStep step, Func<Task> stepInvocation);
    }
}