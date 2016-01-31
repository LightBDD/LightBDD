using System.Collections.Generic;

namespace LightBDD.Core.Metadata.Implementation
{
    internal class ScenarioInfo : IScenarioInfo
    {
        public ScenarioInfo(INameInfo name, IStepInfo[] steps, string[] labels, string[] categories)
        {
            Name = name;
            Steps = steps;
            Labels = labels;
            Categories = categories;
        }

        public INameInfo Name { get; private set; }
        public IEnumerable<IStepInfo> Steps { get; private set; }
        public IEnumerable<string> Labels { get; private set; }
        public IEnumerable<string> Categories { get; private set; }
    }
}