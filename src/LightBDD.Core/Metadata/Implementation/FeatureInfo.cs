using System;
using System.Collections.Generic;
using System.Linq;

namespace LightBDD.Core.Metadata.Implementation
{
    internal class FeatureInfo : IFeatureInfo
    {
        public FeatureInfo(INameInfo name, string[] labels, string description, Type featureType)
        {
            Name = name;
            Labels = labels;
            Description = description;
            FeatureType = featureType;
        }

        public INameInfo Name { get; }
        //TODO: revisit and perhaps encode
        public string RuntimeId => FeatureType.FullName;
        public IEnumerable<string> Labels { get; }
        public string Description { get; }
        public Type FeatureType { get; }

        public override string ToString()
        {
            return Labels.Any()
                ? $"[{string.Join("][", Labels)}] {Name}"
                : Name.ToString();
        }
    }
}