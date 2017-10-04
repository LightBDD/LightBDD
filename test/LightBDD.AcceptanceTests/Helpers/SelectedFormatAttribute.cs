using LightBDD.Framework.Formatting;

namespace LightBDD.AcceptanceTests.Helpers
{
    internal class SelectedFormatAttribute : FormatBooleanAttribute
    {
        public SelectedFormatAttribute()
            : base("selected", "unselected")
        {
        }
    }
}