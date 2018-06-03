using System;
using LightBDD.Framework.Parameters.Implementation;

namespace LightBDD.Framework.Parameters
{
    /// <summary>
    /// Type representing <see cref="Table{TRow}"/> column.
    /// </summary>
    public class TableColumn
    {
        /// <summary>
        /// Column name.
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Function providing column value for specified row object.
        /// </summary>
        public Func<object, ColumnValue> GetValue { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Column name.</param>
        /// <param name="getValue">Function providing column value for specified row object.</param>
        public TableColumn(string name, Func<object, ColumnValue> getValue)
        {
            Name = name;
            GetValue = getValue;
        }

        internal static TableColumn FromColumnInfo(ColumnInfo columnInfo)
        {
            return new TableColumn(columnInfo.Name, columnInfo.GetValue);
        }
    }
}