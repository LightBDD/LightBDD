namespace LightBDD.Formatting.Parameters
{
    /// <summary>
    /// Attribute allowing to define formatting rules for step parameter of boolean type.
    /// </summary>
    public class FormatBooleanAttribute : ParameterFormatterAttribute
    {
        private readonly string _trueValue;
        private readonly string _falseValue;

        /// <summary>
        /// Constructor allowing to associate string constants that would be used for true and false value.
        /// </summary>
        /// <param name="trueValue">String constant used for true value.</param>
        /// <param name="falseValue">String constant used for false value.</param>
        public FormatBooleanAttribute(string trueValue, string falseValue)
        {
            _trueValue = trueValue;
            _falseValue = falseValue;
        }

        /// <summary>
        /// Formats given parameter.
        /// </summary>
        public override string Format(object parameter)
        {
            return ((bool)parameter) ? _trueValue : _falseValue;
        }
    }
}