using System.Diagnostics;
using LightBDD.Core.Results.Parameters.Tabular;

namespace LightBDD.Framework.Results.Implementation
{
    [DebuggerStepThrough]
    internal class TabularParameterColumn : ITabularParameterColumn
    {
        public TabularParameterColumn(string name, bool isKey)
        {
            Name = name;
            IsKey = isKey;
        }

        public string Name { get; }
        public bool IsKey { get; }
    }
}