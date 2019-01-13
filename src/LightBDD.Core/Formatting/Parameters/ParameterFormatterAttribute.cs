using System;
using System.Globalization;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Formatting.Parameters.Implementation;
using LightBDD.Core.Formatting.Values;

namespace LightBDD.Core.Formatting.Parameters
{
    /// <summary>
    /// Parameter formatter attribute, allowing to define custom step parameter formatting method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public abstract class ParameterFormatterAttribute : Attribute, IConditionalValueFormatter, IOrderedAttribute
    {
        /// <summary>
        /// Default implementation accepting any type.
        /// </summary>
        /// <returns>True.</returns>
        public virtual bool CanFormat(Type type)
        {
            return true;
        }
        /// <summary>
        /// Returns format symbols such as null value text representation.
        /// </summary>
        protected IFormatSymbols Symbols => FormatSymbols.Instance;

        /// <inheritdoc />
        public int Order { get; set; }

        /// <summary>
        /// Formats provided <paramref name="value"/> and returns it's string representation.
        /// The provided <paramref name="formattingService"/> can be used to obtain current <see cref="CultureInfo"/> if needed. It can be also used to format inner values if formatted type is collection or complex object.
        /// </summary>
        /// <param name="value">Value to format.</param>
        /// <param name="formattingService">Formatting service allowing to retrieve current <see cref="CultureInfo"/> or format inner values of provided object.</param>
        /// <returns>Formatted value.</returns>
        public abstract string FormatValue(object value, IValueFormattingService formattingService);
    }
}