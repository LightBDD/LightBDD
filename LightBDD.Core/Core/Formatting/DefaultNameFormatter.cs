namespace LightBDD.Core.Formatting
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