namespace LightBDD
{
    /// <summary>
    /// Attribute allowing to define how step parameter should be formatted.
    /// FormatAttribute uses current CultureInfo to format parameters.
    /// </summary>
    public sealed class FormatAttribute : ParameterFormatterAttribute
    {
        private readonly string _format;

        /// <summary>
        /// Constructor allowing to define how step parameter should be formatted.
        /// The <c>format</c> argument represents string.Format() format parameter, where {0} would be a passed parameter instance.
        /// FormatAttribute uses current CultureInfo to format parameters.
        /// </summary>
        public FormatAttribute(string format)
        {
            _format = format;
        }

        /// <summary>
        /// Formats given parameter.
        /// </summary>
        public override string Format(object parameter)
        {
            return string.Format(_format, parameter);
        }
    }
}