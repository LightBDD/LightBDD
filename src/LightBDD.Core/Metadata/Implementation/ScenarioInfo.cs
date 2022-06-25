using System;
using System.Collections.Generic;
using System.Linq;

namespace LightBDD.Core.Metadata.Implementation
{
    internal class ScenarioInfo : IScenarioInfo
    {
        public ScenarioInfo(IFeatureInfo parent, INameInfo name, string[] labels, string[] categories)
        {
            Parent = parent;
            Name = name;
            Labels = labels;
            Categories = categories;
        }

        public INameInfo Name { get; }
        public Guid RuntimeId { get; } = Guid.NewGuid();
        public IFeatureInfo Parent { get; }
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