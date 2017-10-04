using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using LightBDD.Core.Formatting.Values;

namespace LightBDD.Framework.Formatting.Values
{
    /// <summary>
    /// Formatter allowing to format dictionaries implementing <see cref="IDictionary"/> interface.
    /// </summary>
    public class DictionaryFormatter : IConditionalValueFormatter
    {
        private readonly string _containerFormat;
        private readonly string _pairFormat;
        private readonly string _separator;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="containerFormat">Format used to format whole dictionary.</param>
        /// <param name="pairFormat">Format used to format key-value pair.</param>
        /// <param name="separator">Separator used to separate key-value pairs.</param>
        public DictionaryFormatter(string containerFormat = "{0}", string pairFormat = "{0}: {1}", string separator = ", ")
        {
            _containerFormat = containerFormat;
            _pairFormat = pairFormat;
            _separator = separator;
        }

        /// <summary>
        /// Formats <paramref name="value"/> as dictionary, where key-value pairs will be formatted in order based on key.
        /// </summary>
        public string FormatValue(object value, IValueFormattingService formattingService)
        {
            var dictionary = (IDictionary)value;
            var keyValues = dictionary
                .Keys
                .Cast<object>()
                .OrderBy(k => k)
                .Select(key => string.Format(
                    _pairFormat,
                    formattingService.FormatValue(key),
                    formattingService.FormatValue(dictionary[key])));

            return string.Format(_containerFormat, string.Join(_separator, keyValues));
        }

        /// <summary>
        /// Returns true if <paramref name="type"/> implements <see cref="IDictionary"/> type.
        /// </summary>
        public bool CanFormat(Type type)
        {
            return typeof(IDictionary).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo());
        }
    }
}