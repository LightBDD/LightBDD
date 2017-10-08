using System;
using System.Reflection;
using LightBDD.Core.Formatting.Parameters;
using LightBDD.Core.Formatting.Values;

namespace LightBDD.Framework.Formatting
{
    /// <summary>
    /// Attribute allowing to define how step parameter value should be formatted.
    /// </summary>
    public class FormatAttribute : ParameterFormatterAttribute
    {
        private readonly string _format;
        /// <summary>
        /// Constructor allowing to define how step parameter should be formatted.
        /// The <paramref name="format"/> argument represents string.Format() format parameter, where <c>{0}</c> would be a passed parameter instance.
        /// </summary>
        public FormatAttribute(string format)
        {
            _format = format;
        }

        /// <inheritdoc />
        public override string FormatValue(object value, IValueFormattingService formattingService)
        {
            return string.Format(formattingService.GetCultureInfo(), _format, value);
        }

        /// <summary>
        /// Returns true if <paramref name="type"/> is assignable to <see cref="SupportedType"/>.
        /// </summary>
        public override bool CanFormat(Type type)
        {
            return SupportedType.GetTypeInfo().IsAssignableFrom(type.GetTypeInfo());
        }

        /// <summary>
        /// Specifies types for which this formatter is applicable for.
        /// Any type that is assignable to <see cref="SupportedType"/> would be formattable by this formatter.
        /// By default, <see cref="SupportedType"/> is <see cref="object"/>.
        /// </summary>
        public Type SupportedType { get; set; } = typeof(object);
    }
}