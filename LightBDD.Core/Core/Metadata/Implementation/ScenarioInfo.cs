using System.Collections.Generic;
using System.Linq;

namespace LightBDD.Core.Metadata.Implementation
{
    internal class ScenarioInfo : IScenarioInfo
    {
        public ScenarioInfo(INameInfo name, string[] labels, string[] categories)
        {
            Name = name;
            Labels = labels;
            Categories = categories;
        }

        public INameInfo Name { get; }
        public IEnumerable<string> Labels { get; }
        public IEnumerable<string> Categories { get; }

        public override string ToString()
        {
            return Labels.Any()
                ? $"[{string.Join("][", Labels)}] {Name}"
                : Name.ToString();
        }
    }
}