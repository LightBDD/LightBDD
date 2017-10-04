using System;
using LightBDD.Core.Formatting.Parameters;
using LightBDD.Core.Formatting.Values;

namespace LightBDD.Framework.Formatting
{
    /// <summary>
    /// Attribute allowing to define formatting rules for step parameter of boolean type.
    /// </summary>
    public class FormatBooleanAttribute : ParameterFormatterAttribute
    {
        private readonly string _trueValue;
        private readonly string _falseValue;

        /// <summary>
        /// Constructor allowing to associate string constants that would be used for <c>true</c> and <c>false</c> value.
        /// </summary>
        /// <param name="trueValue">String constant used for <c>true</c> value.</param>
        /// <param name="falseValue">String constant used for <c>false</c> value.</param>
        public FormatBooleanAttribute(string trueValue, string falseValue)
        {
            _trueValue = trueValue;
            _falseValue = falseValue;
        }

        public override string FormatValue(object value, IValueFormattingService formattingService)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            return (bool)value ? _trueValue : _falseValue;
        }

        public override bool CanFormat(Type type)
        {
            return type == typeof(bool);
        }
    }
}