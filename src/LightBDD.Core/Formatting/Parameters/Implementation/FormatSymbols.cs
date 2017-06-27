namespace LightBDD.Core.Formatting.Parameters.Implementation
{
    internal class FormatSymbols : IFormatSymbols
    {
        public static IFormatSymbols Instance { get; } = new FormatSymbols();

        private FormatSymbols()
        {
        }

        public string NullValue => "<null>";
    }
}