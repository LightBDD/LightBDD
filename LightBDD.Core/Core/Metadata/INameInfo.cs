using System.Collections.Generic;

namespace LightBDD.Core.Metadata
{
    public interface INameInfo
    {
        string ToString();
        string NameFormat { get; }
        IEnumerable<INameParameterInfo> Parameters{ get; }
    }
}