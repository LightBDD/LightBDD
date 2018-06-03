using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LightBDD.Framework.Expectations;
using LightBDD.Framework.Parameters.Implementation;

namespace LightBDD.Framework.Parameters
{
    /// <summary>
    /// Table extensions allowing to create <see cref="Table{TRow}"/> and <see cref="VerifiableTable{TRow}"/> from collections.
    /// </summary>
    [DebuggerStepThrough]
    public static class TableExtensions
    {
        /// <summary>
        /// Returns <see cref="Table{TRow}"/> with inferred columns and rows specified by <paramref name="items"/> collection.
        /// </summary>
        /// <param name="items">Table rows.</param>
        /// <returns>Table</returns>
        public static Table<T> AsTable<T>(this IEnumerable<T> items)
        {
            var rows = items.ToArray();
            var columns = TableColumnProvider.InferColumns(rows).Select(TableColumn.FromColumnInfo);
            return new Table<T>(columns, rows);
        }

        /// <summary>
        /// Returns <see cref="Table{TRow}"/> with inferred columns and rows specified by <paramref name="items"/> collection.
        /// </summary>
        /// <param name="items">Table rows.</param>
        /// <returns>Table</returns>
        public static Table<KeyValuePair<TKey, TValue>> AsTable<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> items)
        {
            var rows = items.OrderBy(x => x.Key).ToArray();
            var values = rows.Select(x => x.Value).ToArray();
            var valueColumns = TableColumnProvider.InferColumns(values).Select(i => new TableColumn(i.Name,
                pair => i.GetValue(((KeyValuePair<TKey, TValue>)pair).Value)));
            var columns = new[] { new TableColumn("Key", x => ColumnValue.From(((KeyValuePair<TKey, TValue>)x).Key)) }
                .Concat(valueColumns);
            return new Table<KeyValuePair<TKey, TValue>>(columns, rows);
        }

        /// <summary>
        /// Returns <see cref="Table{TRow}"/> defined by <paramref name="tableDefinitionBuilder"/> and rows specified by <paramref name="items"/> collection.
        /// </summary>
        /// <param name="items">Table rows.</param>
        /// <param name="tableDefinitionBuilder">Table definition builder.</param>
        /// <returns>Table</returns>
        public static Table<T> AsTable<T>(this IEnumerable<T> items, Action<ITableBuilder<T>> tableDefinitionBuilder)
        {
            var builder = new TableBuilder<T>();
            tableDefinitionBuilder(builder);
            return builder.Build(items);
        }

        /// <summary>
        /// Returns <see cref="VerifiableTable{TRow}"/> with inferred columns and rows specified by <paramref name="items"/> collection.<br/>
        ///
        /// The created table will have no key columns (the row verification will be index based).<br/>
        /// All columns will use equality expression for verifying values.
        /// </summary>
        /// <param name="items">Table rows.</param>
        /// <returns>Verifiable table</returns>
        public static VerifiableTable<T> AsVerifiableTable<T>(this IEnumerable<T> items)
        {
            var rows = items.ToArray();
            var columns = TableColumnProvider.InferColumns(rows, true).Select(VerifiableTableColumn.FromColumnInfo);
            return new VerifiableTable<T>(columns, rows);
        }

        /// <summary>
        /// Returns <see cref="VerifiableTable{TRow}"/> defined by <paramref name="tableDefinitionBuilder"/> and rows specified by <paramref name="items"/> collection.<br/>
        /// </summary>
        /// <param name="items">Table rows.</param>
        /// <param name="tableDefinitionBuilder">Table definition builder.</param>
        /// <returns>Verifiable table</returns>
        public static VerifiableTable<T> AsVerifiableTable<T>(this IEnumerable<T> items, Action<IVerifiableTableBuilder<T>> tableDefinitionBuilder)
        {
            var builder = new VerifiableTableBuilder<T>();
            tableDefinitionBuilder(builder);
            return builder.Build(items);
        }

        /// <summary>
        /// Returns <see cref="VerifiableTable{TRow}"/> with inferred columns and rows specified by <paramref name="items"/> collection.<br/>
        ///
        /// The created table will use Key column to compare rows.<br/>
        /// All columns will use equality expression for verifying values.
        /// </summary>
        /// <param name="items">Table rows.</param>
        /// <returns>Verifiable table</returns>
        public static VerifiableTable<KeyValuePair<TKey, TValue>> AsVerifiableTable<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> items)
        {
            var rows = items.OrderBy(x => x.Key).ToArray();
            var values = rows.Select(x => x.Value).ToArray();
            var valueColumns = TableColumnProvider.InferColumns(values).Select(i => new VerifiableTableColumn(i.Name, false,
                pair => i.GetValue(((KeyValuePair<TKey, TValue>)pair).Value), expectedColumnValue => Expect.To.Equal(expectedColumnValue)));
            var columns = new[] { new VerifiableTableColumn("Key", true, x => ColumnValue.From(((KeyValuePair<TKey, TValue>)x).Key), expectedColumnValue => Expect.To.Equal(expectedColumnValue)) }
                .Concat(valueColumns);
            return new VerifiableTable<KeyValuePair<TKey, TValue>>(columns, rows);
        }
    }
}