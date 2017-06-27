using System;
using System.Globalization;
using LightBDD.Core.Formatting.Parameters.Implementation;

namespace LightBDD.Core.Formatting.Parameters
{
    /// <summary>
    /// Parameter formatter attribute, allowing to define custom step parameter formatting method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public abstract class ParameterFormatterAttribute : Attribute
    {
        /// <summary>
        /// Formats given <paramref name="parameter"/> value using <paramref name="culture"/>.
        /// </summary>
        /// <param name="culture">Culture used in formatting.</param>
        /// <param name="parameter">Parameter to format.</param>
        /// <returns></returns>
        public abstract string Format(CultureInfo culture, object parameter);

        /// <summary>
        /// Returns format symbols such as null value text representation.
        /// </summary>
        protected IFormatSymbols Symbols => FormatSymbols.Instance;
    }
}