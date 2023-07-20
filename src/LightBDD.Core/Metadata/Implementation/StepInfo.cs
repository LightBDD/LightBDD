using System;

namespace LightBDD.Core.Metadata.Implementation
{
    internal class StepInfo : IStepInfo
    {
        public StepInfo(IMetadataInfo parent, IStepNameInfo name, int number, int total, string groupPrefix)
        {
            Parent = parent;
            Name = name;
            Number = number;
            Total = total;
            GroupPrefix = groupPrefix;
        }

        public IMetadataInfo Parent { get; }
        public string GroupPrefix { get; }
        public int Number { get; }
        public int Total { get; }
        public IStepNameInfo Name { get; private set; }
        public string RuntimeId { get; } = Guid.NewGuid().ToString();

        public void UpdateName(INameParameterInfo[] parameters)
        {
            Name = StepNameInfo.WithUpdatedParameters(Name, parameters);
        }

        public override string ToString()
        {
            return $"{GroupPrefix}{Number}/{GroupPrefix}{Total} {Name}";
        }

        INameInfo IMetadataInfo.Name => Name;
    }
}