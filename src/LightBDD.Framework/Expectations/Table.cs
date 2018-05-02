using System;
using System.Collections.Generic;
using System.Linq;
using LightBDD.Core.Formatting.Parameters;
using LightBDD.Core.Formatting.Values;
using LightBDD.Core.Metadata;
using LightBDD.Core.Results.Parameters;
using LightBDD.Framework.Formatting.Values;
using LightBDD.Framework.Results.Implementation;

namespace LightBDD.Framework.Expectations
{
    public class Table<TRow> : IVerifiableParameter, ISelfFormattable
    {
        private readonly TRow[] _rows;
        private readonly IEnumerable<TableColumn> _columns;
        private IValueFormattingService _formattingService = ValueFormattingServices.Current;

        public Table(IEnumerable<TRow> rows, params TableColumn[] columns)
        {
            _rows = rows.ToArray();
            _columns = columns;
        }

        void IVerifiableParameter.SetValueFormattingService(IValueFormattingService formattingService)
        {
            _formattingService = formattingService;
        }

        public IParameterVerificationResult Result => new TabularParameterResult(GetColumns(), GetRows());

        private IEnumerable<ITabularParameterRow> GetRows()
        {
            return _rows.Select(GetRow);
        }

        private ITabularParameterRow GetRow(TRow row)
        {
            return new TabularParameterRow(_columns.Select(x => _formattingService.FormatValue(x.GetValue(row))));
        }

        private IEnumerable<ITabularParameterColumn> GetColumns()
        {
            return _columns.Select(x => new TabularParameterColumn(x.Name, x.IsKey));
        }

        /// <summary>
        /// Returns inline representation of table
        /// </summary>
        public string Format(IValueFormattingService formattingService)
        {
            return "<table>";
        }

        public class TableColumn
        {
            public TableColumn(string name, bool isKey, Func<TRow, object> getValue)
            {
                Name = name;
                IsKey = isKey;
                GetValue = getValue;
            }

            public string Name { get; }
            public Func<TRow, object> GetValue { get; }
            public bool IsKey { get; }
        }
    }

    internal class TabularParameterRow : ITabularParameterRow
    {
        public TabularParameterRow(IEnumerable<string> values)
        {
            Type = TableRowType.Matching;
            Values = values.Select(x => new ValueResult(x, x, ParameterVerificationStatus.NotApplicable, null))
                .ToArray();
        }

        public TableRowType Type { get; }
        public IEnumerable<IValueResult> Values { get; }
    }

    internal class TabularParameterColumn : ITabularParameterColumn
    {
        public TabularParameterColumn(string name, bool isKey)
        {
            Name = name;
            IsKey = isKey;
        }

        public string Name { get; }
        public bool IsKey { get; }
    }

    public class TabularParameterResult : ITabularParameterResult
    {
        public TabularParameterResult(IEnumerable<ITabularParameterColumn> columns, IEnumerable<ITabularParameterRow> rows)
        {
            Columns = columns.ToArray();
            Rows = rows.ToArray();
        }

        public Exception Exception { get; } = null;
        public ParameterVerificationStatus VerificationStatus { get; } = ParameterVerificationStatus.NotApplicable;
        public IEnumerable<ITabularParameterColumn> Columns { get; }
        public IEnumerable<ITabularParameterRow> Rows { get; }
    }

    public static class Ext
    {
        public static Table<KeyValuePair<TKey, TValue>> AsTable<TKey, TValue>(
            this IDictionary<TKey, TValue> dictionary)
        {
            return new Table<KeyValuePair<TKey, TValue>>(dictionary,
                new Table<KeyValuePair<TKey, TValue>>.TableColumn("Key", true, r => r.Key),
                new Table<KeyValuePair<TKey, TValue>>.TableColumn("Value", false, r => r.Value));
        }
    }
}