using System;

namespace LightBDD.Framework.Parameters.Implementation
{
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