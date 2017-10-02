using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using LightBDD.Core.Formatting.Values;

namespace LightBDD.Framework.Formatting.Values
{
    public class DictionaryFormatter : IConditionalValueFormatter
    {
        public string FormatValue(object value, IValueFormattingService formattingService)
        {
            var dictionary = (IDictionary)value;
            var keyValues = dictionary
                .Keys
                .Cast<object>()
                .OrderBy(k => k)
                .Select(key => string.Format(
                    "{{{0}: {1}}}",
                    formattingService.FormatValue(key),
                    formattingService.FormatValue(dictionary[key])));

            return string.Join(", ", keyValues);
        }

        public bool CanFormat(Type type)
        {
            return typeof(IDictionary).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo());
        }
    }
}