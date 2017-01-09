using System.Collections.Generic;
using LightBDD.Core.Formatting.NameDecorators;

namespace LightBDD.Core.Metadata
{
    public interface INameInfo
    {
        string ToString();
        string Format(INameDecorator decorator);
        string NameFormat { get; }
        IEnumerable<INameParameterInfo> Parameters { get; }
    }
}