using System.Collections.Generic;

namespace LightBDD.Core.Metadata
{
    public interface IScenarioInfo
    {
        INameInfo Name { get; }
        IEnumerable<string> Labels { get; }
        IEnumerable<string> Categories { get; }
    }
}