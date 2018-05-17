using System;

namespace LightBDD.Framework.Parameters
{
    public class TableColumn
    {
        public TableColumn(string name, Func<object, ColumnValue> getValue)
        {
            Name = name;
            GetValue = getValue;
        }

        public string Name { get; }
        public Func<object, ColumnValue> GetValue { get; }
    }
}