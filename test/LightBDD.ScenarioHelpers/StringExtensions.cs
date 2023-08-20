namespace LightBDD.ScenarioHelpers
{
    public static class StringExtensions
    {
        public static string NormalizeNewLine(this string txt) => txt.Replace("\r", "");
    }
}