using System;
using LightBDD.Framework.Parameters.Implementation;

namespace LightBDD.Framework.Parameters
{
    /// <summary>
    /// Type representing <see cref="InputTable{TRow}"/> column.
    /// </summary>
    public class InputTableColumn
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
        public InputTableColumn(string name, Func<object, ColumnValue> getValue)
        {
            Name = name;
            GetValue = getValue;
        }

        internal static InputTableColumn FromColumnInfo(ColumnInfo columnInfo)
        {
            return new InputTableColumn(columnInfo.Name, columnInfo.GetValue);
        }
    }
}