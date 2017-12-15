using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace LightBDD.Core.Metadata.Implementation
{
    [DebuggerStepThrough]
    internal class FeatureInfo : IFeatureInfo
    {
        public FeatureInfo(string runtimeId, INameInfo name, string[] labels, string description)
        {
            RuntimeId = runtimeId;
            Name = name;
            Labels = labels;
            Description = description;
        }

        public string RuntimeId { get; }
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