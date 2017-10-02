using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using LightBDD.Core.Formatting.Values;

namespace LightBDD.Framework.Formatting.Values
{
    public class CollectionFormatter : IConditionalValueFormatter
    {
        public string FormatValue(object value, IValueFormattingService formattingService)
        {
            return string.Join(", ", ((IEnumerable) value).Cast<object>().Select(formattingService.FormatValue));
        }

        public bool CanFormat(Type type)
        {
            return typeof(IEnumerable).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo());
        }
    }
}