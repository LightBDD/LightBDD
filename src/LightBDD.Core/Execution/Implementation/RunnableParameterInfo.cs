using System;
using LightBDD.Core.Metadata;

namespace LightBDD.Core.Execution.Implementation
{
    internal class RunnableParameterInfo : IParameterInfo
    {
        public RunnableParameterInfo(IMetadataInfo owner, string name)
        {
            Owner = owner;
            Name = name;
        }

        public Guid RuntimeId { get; } = Guid.NewGuid();
        public IMetadataInfo Owner { get; }
        public string Name { get; }
    }
}