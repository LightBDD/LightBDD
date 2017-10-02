using System;
using System.Globalization;
using System.Reflection;
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
        /// Formats given <paramref name="parameter"/> value using <paramref name="culture"/>.
        /// </summary>
        /// <param name="culture">Culture used in formatting.</param>
        /// <param name="parameter">Parameter to format.</param>
        /// <returns>Formatted parameter</returns>
        [Obsolete("Use " + nameof(FormatValue) + " instead")]
        public virtual string Format(CultureInfo culture, object parameter){throw new NotImplementedException();}

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

        public virtual string FormatValue(object value, IValueFormattingService formattingService)
        {
#pragma warning disable 618
            return Format(formattingService.GetCultureInfo(), value);
#pragma warning restore 618
        }
    }
}