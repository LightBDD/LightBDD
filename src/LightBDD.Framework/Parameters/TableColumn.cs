using System;

namespace LightBDD.Framework.Parameters
{
    public class TableColumn
    {
        public TableColumn(string name, bool isKey, Func<object, ColumnValue> getValue)
        {
            Name = name;
            IsKey = isKey;
            GetValue = getValue;
        }

        public string Name { get; }
        public Func<object, ColumnValue> GetValue { get; }
        public bool IsKey { get; }
    }
}