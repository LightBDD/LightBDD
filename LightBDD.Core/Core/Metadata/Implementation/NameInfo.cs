using System.Collections.Generic;
using System.Linq;

namespace LightBDD.Core.Metadata.Implementation
{
    internal class NameInfo : INameInfo
    {
        public string NameFormat { get; }

        public IEnumerable<INameParameterInfo> Parameters { get; }

        public NameInfo(string nameFormat, INameParameterInfo[] parameters)
        {
            NameFormat = nameFormat;
            Parameters = parameters;
        }

        public override string ToString()
        {
            if (!Parameters.Any())
                return NameFormat;
            return string.Format(NameFormat, Parameters.Select(p => (object)p.FormattedValue).ToArray());
        }
    }
}