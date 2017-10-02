using System;

namespace LightBDD.Core.Formatting.Values
{
    public interface IConditionalValueFormatter : IValueFormatter
    {
        bool CanFormat(Type type);
    }
}