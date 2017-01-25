using System.Collections.Generic;

namespace LightBDD.Core.Metadata
{
    public interface IFeatureInfo
    {
        INameInfo Name { get; }
        IEnumerable<string> Labels { get; }
        string Description { get; }
    }
}