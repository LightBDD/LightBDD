using System;
using LightBDD.Framework.Parameters.Implementation;

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

        internal static TableColumn FromColumnInfo(ColumnInfo columnInfo)
        {
            return new TableColumn(columnInfo.Name, columnInfo.GetValue);
        }
    }
}