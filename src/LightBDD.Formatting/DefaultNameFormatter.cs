using LightBDD.Core.Formatting;

namespace LightBDD.Formatting
{
    public class DefaultNameFormatter : INameFormatter
    {
        public string FormatName(string name)
        {
            return name
                .Replace("__", ": ")
                .Replace("_s_", "'s ")
                .Replace("_", " ");
        }
    }
}