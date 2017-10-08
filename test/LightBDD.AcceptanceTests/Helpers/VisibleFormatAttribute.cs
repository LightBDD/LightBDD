using LightBDD.Framework.Formatting;

namespace LightBDD.AcceptanceTests.Helpers
{
    internal class VisibleFormatAttribute : FormatBooleanAttribute
    {
        public VisibleFormatAttribute()
            : base("visible", "invisible")
        {
        }
    }
}