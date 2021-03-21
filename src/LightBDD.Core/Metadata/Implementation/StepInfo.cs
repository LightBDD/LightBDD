using System;
using System.Collections.Generic;

namespace LightBDD.Core.Metadata.Implementation
{
    internal class StepInfo : IStepInfo
    {
        private readonly StepNameInfo _name;

        public StepInfo(IMetadataInfo parent, StepNameInfo name, int number, int total, string groupPrefix)
        {
            Parent = parent;
            _name = name;
            Number = number;
            Total = total;
            GroupPrefix = groupPrefix;
        }

        public IMetadataInfo Parent { get; }
        public string GroupPrefix { get; }
        public int Number { get; }
        public int Total { get; }

        public IStepNameInfo Name => _name;
        INameInfo IMetadataInfo.Name => Name;
        public Guid RuntimeId { get; } = Guid.NewGuid();

        public void UpdateName(IReadOnlyList<INameParameterInfo> parameters)
        {
            _name.UpdateParameters(parameters);
        }

        public override string ToString()
        {
            return $"{GroupPrefix}{Number}/{GroupPrefix}{Total} {Name}";
        }
    }
}