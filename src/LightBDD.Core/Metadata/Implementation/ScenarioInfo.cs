using System;
using System.Collections.Generic;
using System.Linq;

namespace LightBDD.Core.Metadata.Implementation
{
    internal class ScenarioInfo : IScenarioInfo
    {
        private readonly NameInfo _name;

        public ScenarioInfo(IFeatureInfo parent, NameInfo name, string[] labels, string[] categories)
        {
            Parent = parent;
            _name = name;
            Labels = labels;
            Categories = categories;
        }

        public INameInfo Name => _name;
        public Guid RuntimeId { get; } = Guid.NewGuid();
        public IFeatureInfo Parent { get; }
        public IEnumerable<string> Labels { get; }
        public IEnumerable<string> Categories { get; }

        public void UpdateName(IReadOnlyList<INameParameterInfo> parameters)
        {
            _name.UpdateParameters(parameters);
        }

        public override string ToString()
        {
            return Labels.Any()
                ? $"[{string.Join("][", Labels)}] {Name}"
                : Name.ToString();
        }
    }
}