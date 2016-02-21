using System.Collections.Generic;
using System.Threading.Tasks;

namespace LightBDD.Core.Extensibility
{
    public interface IScenarioRunner
    {
        IScenarioRunner WithSteps(IEnumerable<StepDescriptor> steps);
        IScenarioRunner WithCapturedScenarioDetails();
        IScenarioRunner WithLabels(string[] labels);
        IScenarioRunner WithCategories(string[] categories);
        IScenarioRunner WithName(string name);
        Task RunAsynchronously();
        void RunSynchronously();
    }
}