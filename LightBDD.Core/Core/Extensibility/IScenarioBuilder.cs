using System.Collections.Generic;
using LightBDD.Core.Metadata;

namespace LightBDD.Core.Extensibility
{
    public interface IScenarioBuilder
    {
        IScenarioBuilder WithSteps(IEnumerable<StepDescriptor> steps);
        IScenarioBuilder WithCapturedScenarioDetails();
        IScenarioInfo Build();
        IScenarioBuilder WithLabels(string[] labels);
        IScenarioBuilder WithCategories(string[] categories);
    }
}