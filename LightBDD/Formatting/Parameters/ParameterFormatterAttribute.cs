using System;

namespace LightBDD.Formatting.Parameters
{
    /// <summary>
    /// Parameter formatter attribute, allowing to define custom step parameter formatting method
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public abstract class ParameterFormatterAttribute : Attribute
    {
        /// <summary>
        /// Formats given parameter.
        /// </summary>
        public abstract string Format(object parameter);
    }
}