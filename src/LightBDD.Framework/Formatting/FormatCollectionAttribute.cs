using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using LightBDD.Core.Formatting.Parameters;
using LightBDD.Core.Formatting.Values;

namespace LightBDD.Framework.Formatting
{
    /// <summary>
    /// Attribute allowing to define formatting rules for method parameter of type implementing <see cref="IEnumerable"/> interface.
    /// </summary>
    public class FormatCollectionAttribute : ParameterFormatterAttribute
    {
        private readonly string _separator;
        private readonly string _valueFormat;

        /// <summary>
        /// Constructor allowing to specify separator and value format.
        /// </summary>
        /// <param name="separator">Value separator. If <c>null</c>, <c>", "</c> will be used.</param>
        /// <param name="valueFormat">Value format. If <c>null</c>, <c>"{0}"</c> will be used.</param>
        public FormatCollectionAttribute(string separator = null, string valueFormat = null)
        {
            _separator = separator ?? ", ";
            _valueFormat = valueFormat ?? "{0}";
        }

        /// <summary>
        /// Returns true if <paramref name="type"/> implements <see cref="IEnumerable"/> type.
        /// </summary>
        public override bool CanFormat(Type type)
        {
            return typeof(IEnumerable).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()) && type != typeof(string);
        }

        /// <summary>
        /// Formats provided <paramref name="value"/> as collection.
        /// </summary>
        public override string FormatValue(object value, IValueFormattingService formattingService)
        {
            return string.Join(_separator, ((IEnumerable)value).Cast<object>().Select(o => string.Format(_valueFormat, formattingService.FormatValue(o))));
        }
    }
}