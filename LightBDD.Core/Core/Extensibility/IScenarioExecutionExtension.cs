using System;
using System.Threading.Tasks;
using LightBDD.Core.Metadata;

namespace LightBDD.Core.Extensibility
{
    //TODO: add tests
    public interface IScenarioExecutionExtension
    {
        Task ExecuteAsync(IScenarioInfo scenario, Func<Task> scenarioInvocation);
    }
}