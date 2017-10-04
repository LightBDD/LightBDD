using LightBDD.Framework.Formatting;

namespace LightBDD.AcceptanceTests.Helpers
{
    internal class OrderFormatAttribute : FormatBooleanAttribute
    {
        public OrderFormatAttribute()
            : base("ascending", "descending")
        {
        }
    }
}