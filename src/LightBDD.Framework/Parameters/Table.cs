using System;
using System.Collections.Generic;
using System.Linq;
using LightBDD.Framework.Expectations;
using LightBDD.Framework.Parameters.Implementation;

namespace LightBDD.Framework.Parameters
{
    /// <summary>
    /// Static class allowing to create various types of table parameters.
    /// </summary>
    public static class Table
    {
        /// <summary>
        /// Returns <see cref="InputTable{TRow}"/> with inferred columns and rows specified by <paramref name="items"/> collection.
        /// </summary>
        /// <param name="items">Table rows.</param>
        /// <returns>Table</returns>
        public static InputTable<TRow> ToTable<TRow>(this IEnumerable<TRow> items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));
            return For(items.ToArray());
        }

        /// <summary>
        /// Returns <see cref="InputTable{TRow}"/> with inferred columns and rows specified by <paramref name="items"/> collection.
        /// </summary>
        /// <param name="items">Table rows.</param>
        /// <returns>Table</returns>
        public static InputTable<KeyValuePair<TKey, TValue>> ToTable<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> items)
        {
            var rows = items.OrderBy(x => x.Key).ToArray();
            var values = rows.Select(x => x.Value).ToArray();
            var valueColumns = TableColumnProvider.InferColumns(values).Select(i => new InputTableColumn(i.Name,
                pair => i.GetValue(((KeyValuePair<TKey, TValue>)pair).Value)));
            var columns = new[] { new InputTableColumn("Key", x => ColumnValue.From(((KeyValuePair<TKey, TValue>)x).Key)) }
                .Concat(valueColumns);
            return new InputTable<KeyValuePair<TKey, TValue>>(columns, rows);
        }

        /// <summary>
        /// Returns <see cref="InputTable{TRow}"/> defined by <paramref name="definitionBuilder"/> and rows specified by <paramref name="items"/> collection.
        /// </summary>
        /// <param name="items">Table rows.</param>
        /// <param name="definitionBuilder">Table definition builder.</param>
        /// <returns>Table</returns>
        public static InputTable<T> ToTable<T>(this IEnumerable<T> items, Action<IInputTableBuilder<T>> definitionBuilder)
        {
            var builder = new InputTableBuilder<T>();
            definitionBuilder(builder);
            return builder.Build(items);
        }

        /// <summary>
        /// Returns <see cref="InputTable{TRow}"/> with inferred columns and rows specified by <paramref name="items"/> collection.
        /// </summary>
        /// <param name="items">Table rows.</param>
        /// <returns>Table</returns>
        public static InputTable<TRow> For<TRow>(params TRow[] items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));
            var columns = TableColumnProvider.InferColumns(items).Select(InputTableColumn.FromColumnInfo);
            return new InputTable<TRow>(columns, items);
        }

        /// <summary>
        /// Returns <see cref="InputTable{TRow}"/> defined by <paramref name="definitionBuilder"/> and rows specified by <paramref name="items"/> collection.
        /// </summary>
        /// <param name="items">Table rows.</param>
        /// <param name="definitionBuilder">Table definition builder.</param>
        /// <returns>Table</returns>
        public static InputTable<T> For<T>(Action<IInputTableBuilder<T>> definitionBuilder, params T[] items)
        {
            var builder = new InputTableBuilder<T>();
            definitionBuilder(builder);
            return builder.Build(items);
        }

        /// <summary>
        /// Returns <see cref="TableValidator{TRow}"/> defined by <paramref name="definitionBuilder"/>.
        /// </summary>
        /// <param name="definitionBuilder">Definition builder.</param>
        /// <returns></returns>
        public static TableValidator<TRow> Validate<TRow>(Action<ITableValidatorBuilder<TRow>> definitionBuilder)
        {
            var builder = new TableValidatorBuilder<TRow>();
            definitionBuilder(builder);
            return builder.Build();
        }

        /// <summary>
        /// Returns <see cref="VerifiableDataTable{TRow}"/> with inferred columns and rows specified by <paramref name="items"/> collection.<br/>
        ///
        /// The created table will have no key columns (the row verification will be index based).<br/>
        /// All columns will use equality expression for verifying values.
        /// </summary>
        /// <param name="items">Table rows.</param>
        /// <returns>Verifiable table</returns>
        public static VerifiableDataTable<TRow> ExpectData<TRow>(params TRow[] items)
        {
            var columns = TableColumnProvider.InferColumns(items, true).Select(VerifiableTableColumn.FromColumnInfo);
            return new VerifiableDataTable<TRow>(columns, items);
        }

        /// <summary>
        /// Returns <see cref="VerifiableDataTable{TRow}"/> defined by <paramref name="definitionBuilder"/> and rows specified by <paramref name="items"/> collection.<br/>
        /// </summary>
        /// <param name="items">Table rows.</param>
        /// <param name="definitionBuilder">Table definition builder.</param>
        /// <returns>Verifiable table</returns>
        public static VerifiableDataTable<TRow> ExpectData<TRow>(Action<IVerifiableDataTableBuilder<TRow>> definitionBuilder, params TRow[] items)
        {
            var builder = new VerifiableDataTableBuilder<TRow>();
            definitionBuilder(builder);
            return builder.Build(items);
        }

        /// <summary>
        /// Returns <see cref="VerifiableDataTable{TRow}"/> with inferred columns and rows specified by <paramref name="items"/> collection.<br/>
        ///
        /// The created table will have no key columns (the row verification will be index based).<br/>
        /// All columns will use equality expression for verifying values.
        /// </summary>
        /// <param name="items">Table rows.</param>
        /// <returns>Verifiable table</returns>
        public static VerifiableDataTable<T> ToVerifiableDataTable<T>(this IEnumerable<T> items)
        {
            return ExpectData(items.ToArray());
        }

        /// <summary>
        /// Returns <see cref="VerifiableDataTable{TRow}"/> defined by <paramref name="definitionBuilder"/> and rows specified by <paramref name="items"/> collection.<br/>
        /// </summary>
        /// <param name="items">Table rows.</param>
        /// <param name="definitionBuilder">Table definition builder.</param>
        /// <returns>Verifiable table</returns>
        public static VerifiableDataTable<T> ToVerifiableDataTable<T>(this IEnumerable<T> items, Action<IVerifiableDataTableBuilder<T>> definitionBuilder)
        {
            var builder = new VerifiableDataTableBuilder<T>();
            definitionBuilder(builder);
            return builder.Build(items);
        }

        /// <summary>
        /// Returns <see cref="VerifiableDataTableBuilder{TRow}"/> with inferred columns and rows specified by <paramref name="items"/> collection.<br/>
        ///
        /// The created table will use Key column to compare rows.<br/>
        /// All columns will use equality expression for verifying values.
        /// </summary>
        /// <param name="items">Table rows.</param>
        /// <returns>Verifiable table</returns>
        public static VerifiableDataTable<KeyValuePair<TKey, TValue>> ToVerifiableDataTable<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> items)
        {
            var rows = items.OrderBy(x => x.Key).ToArray();
            var values = rows.Select(x => x.Value).ToArray();
            var valueColumns = TableColumnProvider.InferColumns(values).Select(i => new VerifiableTableColumn(i.Name,
                false,
                pair => i.GetValue(((KeyValuePair<TKey, TValue>)pair).Value), Expect.To.Equal));
            var columns = new[]
                {
                    new VerifiableTableColumn("Key", true, x => ColumnValue.From(((KeyValuePair<TKey, TValue>) x).Key),
                        Expect.To.Equal)
                }
                .Concat(valueColumns);
            return new VerifiableDataTable<KeyValuePair<TKey, TValue>>(columns, rows);
        }
    }
}
