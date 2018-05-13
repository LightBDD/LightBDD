using LightBDD.Core.Formatting.Values;

namespace LightBDD.Framework.Parameters
{
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

        public override string ToString()
        {
            return HasValue ? Value?.ToString() : "<no value>";
        }
    }
}