using System.Collections.Generic;

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
    }
}