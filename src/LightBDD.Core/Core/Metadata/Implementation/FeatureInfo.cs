using System.Collections.Generic;
using System.Linq;

namespace LightBDD.Core.Metadata.Implementation
{
    internal class FeatureInfo : IFeatureInfo
    {
        public FeatureInfo(INameInfo name, string[] labels, string description)
        {
            Name = name;
            Labels = labels;
            Description = description;
        }

        public INameInfo Name { get; }
        public IEnumerable<string> Labels { get; }
        public string Description { get; }

        public override string ToString()
        {
            return Labels.Any()
                ? $"[{string.Join("][", Labels)}] {Name}"
                : Name.ToString();
        }
    }
}