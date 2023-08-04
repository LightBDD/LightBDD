#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

namespace LightBDD.Core.Metadata.Implementation
{
    internal class ScenarioInfo : IScenarioInfo
    {
        public ScenarioInfo(IFeatureInfo parent, INameInfo name, IReadOnlyList<string> labels, IReadOnlyList<string> categories, string? runtimeId)
        {
            Parent = parent;
            Name = name;
            Labels = labels;
            Categories = categories;
            RuntimeId = runtimeId ?? Guid.NewGuid().ToString();
        }

        public INameInfo Name { get; }
        public string RuntimeId { get; }
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