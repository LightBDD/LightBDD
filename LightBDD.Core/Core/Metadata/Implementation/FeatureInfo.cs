using System.Collections.Generic;
using System.Linq;

namespace LightBDD.Core.Metadata.Implementation
{
    class FeatureInfo : IFeatureInfo
    {
        public FeatureInfo(INameInfo name, string[] labels, string description)
        {
            Name = name;
            Labels = labels;
            Description = description;
        }

        public INameInfo Name { get; private set; }
        public IEnumerable<string> Labels { get; private set; }
        public string Description { get; private set; }

        public override string ToString()
        {
            return Labels.Any()
                ? $"[{string.Join("][", Labels)}] {Name}"
                : Name.ToString();
        }
    }
}