using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using LightBDD.Core.Formatting.Values;

namespace LightBDD.Framework.Formatting.Values
{
    public class DictionaryFormatter : IConditionalValueFormatter
    {
        private readonly string _containerFormat;
        private readonly string _pairFormat;
        private readonly string _separator;

        public DictionaryFormatter(string containerFormat = "{0}", string pairFormat = "{0}: {1}", string separator = ", ")
        {
            _containerFormat = containerFormat;
            _pairFormat = pairFormat;
            _separator = separator;
        }

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

        public bool CanFormat(Type type)
        {
            return typeof(IDictionary).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo());
        }
    }
}