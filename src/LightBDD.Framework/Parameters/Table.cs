using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LightBDD.Core.Execution;
using LightBDD.Core.Formatting.Values;
using LightBDD.Core.Results.Parameters;
using LightBDD.Core.Results.Parameters.Tabular;
using LightBDD.Framework.Formatting.Values;
using LightBDD.Framework.Parameters.Implementation;
using LightBDD.Framework.Results.Implementation;

namespace LightBDD.Framework.Parameters
{
    /// <summary>
    /// Table extensions allowing to create <see cref="Table{TRow}"/>.
    /// </summary>
    [DebuggerStepThrough]
    public static class Table
    {
        /// <summary>
        /// Returns <see cref="Table{TRow}"/> with inferred columns and rows specified by <paramref name="items"/> collection.
        /// </summary>
        /// <param name="items">Table rows.</param>
        /// <returns>Table</returns>
        public static Table<T> ToTable<T>(this IEnumerable<T> items)
        {
            return CreateFor(items.ToArray());
        }

        /// <summary>
        /// Returns <see cref="Table{TRow}"/> with inferred columns and rows specified by <paramref name="items"/> collection.
        /// </summary>
        /// <param name="items">Table rows.</param>
        /// <returns>Table</returns>
        public static Table<T> CreateFor<T>(params T[] items)
        {
            var columns = TableColumnProvider.InferColumns(items).Select(TableColumn.FromColumnInfo);
            return new Table<T>(columns, items);
        }

        /// <summary>
        /// Returns <see cref="Table{TRow}"/> with inferred columns and rows specified by <paramref name="items"/> collection.
        /// </summary>
        /// <param name="items">Table rows.</param>
        /// <returns>Table</returns>
        public static Table<KeyValuePair<TKey, TValue>> ToTable<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> items)
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
        public static Table<T> ToTable<T>(this IEnumerable<T> items, Action<ITableBuilder<T>> tableDefinitionBuilder)
        {
            var builder = new TableBuilder<T>();
            tableDefinitionBuilder(builder);
            return builder.Build(items);
        }

        /// <summary>
        /// Returns <see cref="Table{TRow}"/> defined by <paramref name="tableDefinitionBuilder"/> and rows specified by <paramref name="items"/> collection.
        /// </summary>
        /// <param name="items">Table rows.</param>
        /// <param name="tableDefinitionBuilder">Table definition builder.</param>
        /// <returns>Table</returns>
        public static Table<T> CreateFor<T>(Action<ITableBuilder<T>> tableDefinitionBuilder, params T[] items)
        {
            var builder = new TableBuilder<T>();
            tableDefinitionBuilder(builder);
            return builder.Build(items);
        }
    }

    /// <summary>
    /// Type representing tabular step parameter.
    /// When used in step methods, the tabular representation of the parameter will be rendered in reports and progress notification.<br/>
    ///
    /// Beside special rendering, the table behaves as a standard collection, i.e it offers <see cref="Count"/>, <see cref="GetEnumerator()"/> and indexing operator members.<br/>
    ///
    /// Please see <see cref="Table"/> type to learn how to create tables effectively.
    /// </summary>
    /// <typeparam name="TRow">Row type.</typeparam>
    [DebuggerStepThrough]
    public class Table<TRow> : IComplexParameter, ISelfFormattable, IReadOnlyList<TRow>
    {
        private readonly IReadOnlyList<TRow> _rows;
        private readonly TableColumn[] _columns;
        private IValueFormattingService _formattingService = ValueFormattingServices.Current;

        /// <summary>
        /// Constructor creating table with specified <paramref name="columns"/> and <paramref name="rows"/>
        /// </summary>
        /// <param name="columns">Table columns.</param>
        /// <param name="rows">Table rows.</param>
        public Table(IEnumerable<TableColumn> columns, IReadOnlyList<TRow> rows)
        {
            _rows = rows;
            _columns = columns.ToArray();
        }

        void IComplexParameter.SetValueFormattingService(IValueFormattingService formattingService)
        {
            _formattingService = formattingService;
        }

        IParameterDetails IComplexParameter.Details => new TabularParameterDetails(GetColumns(), GetRows());

        private IEnumerable<ITabularParameterRow> GetRows()
        {
            return _rows.Select(GetRow);
        }

        private ITabularParameterRow GetRow(TRow row, int index)
        {
            return new TabularParameterRow(index, _columns.Select(x => _formattingService.FormatValue(x.GetValue(row))));
        }

        private IEnumerable<ITabularParameterColumn> GetColumns()
        {
            return _columns.Select(x => new TabularParameterColumn(x.Name, false));
        }

        /// <summary>
        /// Returns inline representation of table.
        /// </summary>
        public string Format(IValueFormattingService formattingService)
        {
            return "<table>";
        }

        /// <summary>
        /// Returns number of rows.
        /// </summary>
        public int Count => _rows.Count;

        /// <summary>
        /// Returns enumerator for the table rows.
        /// </summary>
        public IEnumerator<TRow> GetEnumerator()
        {
            return _rows.AsEnumerable().GetEnumerator();
        }

        /// <summary>
        /// Returns row at specified <paramref name="index"/>.
        /// </summary>
        /// <param name="index">Row index</param>
        public TRow this[int index] => _rows[index];

        /// <summary>
        /// Returns columns collection.
        /// </summary>
        public IReadOnlyList<TableColumn> Columns => _columns;

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }


}