using System;
using System.Collections.Generic;
using System.Text;

namespace LightBDD.Core.Metadata
{
    public interface IParameterInfo
    {
        Guid RuntimeId { get; }
        IMetadataInfo Owner { get; }
        string Name { get; }
    }
}
