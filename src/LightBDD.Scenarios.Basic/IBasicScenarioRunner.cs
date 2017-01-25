using System;
using System.Threading.Tasks;

namespace LightBDD.Scenarios.Basic
{
    public interface IBasicScenarioRunner
    {
        void RunScenario(params Action[] steps);
        Task RunScenarioAsync(params Func<Task>[] steps);
    }
}