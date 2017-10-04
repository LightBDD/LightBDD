using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using LightBDD.Core.Formatting.Values;

namespace LightBDD.Framework.Formatting.Values
{
    /// <summary>
    /// Formatter allowing to format collections that implements <see cref="IEnumerable"/> interface.
    /// </summary>
    public class CollectionFormatter : IConditionalValueFormatter
    {
        private readonly string _containerFormat;
        private readonly string _separator;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="containerFormat">Format used to format whole collection.</param>
        /// <param name="separator">Separator used to separate collection items.</param>
        public CollectionFormatter(string containerFormat = "{0}", string separator = ", ")
        {
            _containerFormat = containerFormat;
            _separator = separator;
        }

        /// <summary>
        /// Formats <paramref name="value"/> as collection.
        /// </summary>
        public string FormatValue(object value, IValueFormattingService formattingService)
        {
            return string.Format(_containerFormat, string.Join(_separator, ((IEnumerable)value).Cast<object>().Select(formattingService.FormatValue)));
        }

        /// <summary>
        /// Returns true if <paramref name="type"/> implements <see cref="IEnumerable"/> type.
        /// </summary>
        public bool CanFormat(Type type)
        {
            return typeof(IEnumerable).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo());
        }
    }
}