using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using LightBDD.Core.Formatting.Values;

namespace LightBDD.Framework.Formatting.Values
{
    public class CollectionFormatter : IConditionalValueFormatter
    {
        private readonly string _containerFormat;
        private readonly string _separator;

        public CollectionFormatter(string containerFormat = "{0}", string separator = ", ")
        {
            _containerFormat = containerFormat;
            _separator = separator;
        }

        public string FormatValue(object value, IValueFormattingService formattingService)
        {
            return string.Format(_containerFormat, string.Join(_separator, ((IEnumerable)value).Cast<object>().Select(formattingService.FormatValue)));
        }

        public bool CanFormat(Type type)
        {
            return typeof(IEnumerable).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo());
        }
    }
}