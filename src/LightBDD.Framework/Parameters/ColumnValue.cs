using LightBDD.Core.Formatting.Values;
using LightBDD.Framework.Formatting.Values;

namespace LightBDD.Framework.Parameters
{
    /// <summary>
    /// Type representing column value.
    /// </summary>
    public struct ColumnValue : ISelfFormattable
    {
        /// <summary>
        /// Returns true if value is specified.
        /// False means that this column value does not exists for the given row (like there was no property defined on the object etc.).
        /// Please note that if column value exists but it's null, the <see cref="HasValue"/> should be true and <see cref="Value"/> should be null.
        /// </summary>
        public bool HasValue { get; }
        /// <summary>
        /// Returns provided column value, or null if no value is specified (<see cref="HasValue"/> is false).
        /// </summary>
        public object Value { get; }

        private ColumnValue(object value)
        {
            Value = value;
            HasValue = true;
        }

        /// <summary>
        /// Represents no value, which means that for given row, this column value does not exists.
        /// </summary>
        public static readonly ColumnValue None = new();

        /// <summary>
        /// Creates column value object with <see cref="HasValue"/> equal true and <see cref="Value"/> equal <paramref name="value"/>.
        /// </summary>
        /// <param name="value">Value.</param>
        public static ColumnValue From(object value)
        {
            return new ColumnValue(value);
        }

        /// <inheritdoc />
        public string Format(IValueFormattingService formattingService)
        {
            return HasValue ? formattingService.FormatValue(Value) : "<none>";
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return Format(ValueFormattingServices.Current);
        }
    }
}