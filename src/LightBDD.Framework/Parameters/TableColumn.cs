using System;
using LightBDD.Core.Formatting.Values;

namespace LightBDD.Framework.Parameters
{
    public class TableColumn<TRow>
    {
        public TableColumn(string name, bool isKey, Func<TRow, ColumnValue> getValue)
        {
            Name = name;
            IsKey = isKey;
            GetValue = getValue;
        }

        public string Name { get; }
        public Func<TRow, ColumnValue> GetValue { get; }
        public bool IsKey { get; }
    }

    public struct ColumnValue : ISelfFormattable
    {
        public bool HasValue { get; }
        public object Value { get; }

        public ColumnValue(object value)
        {
            Value = value;
            HasValue = true;
        }

        public static readonly ColumnValue None = new ColumnValue();

        public static ColumnValue From(object value)
        {
            return new ColumnValue(value);
        }

        public string Format(IValueFormattingService formattingService)
        {
            return HasValue ? formattingService.FormatValue(Value) : "<none>";
        }
    }
}