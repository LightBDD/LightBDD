using System;
using System.Diagnostics;

namespace LightBDD.Framework.Parameters.Implementation
{
    [DebuggerStepThrough]
    internal struct ColumnInfo
    {
        public ColumnInfo(string name, Func<object, ColumnValue> getValue)
        {
            Name = name;
            GetValue = getValue;
        }

        public string Name { get; }
        public Func<object, ColumnValue> GetValue { get; }
    }
}