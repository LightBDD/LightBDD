using System;
using System.Reflection;

namespace LightBDD.Core.Formatting.Values.Implementation
{
    internal class SelfFormattable : IConditionalValueFormatter
    {
        public static SelfFormattable Instance { get; } = new();
        public string FormatValue(object value, IValueFormattingService formattingService)
        {
            return ((ISelfFormattable)value).Format(formattingService);
        }

        public bool CanFormat(Type type)
        {
            return typeof(ISelfFormattable).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo());
        }
    }
}