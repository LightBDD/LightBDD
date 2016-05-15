namespace LightBDD.UnitTests.Helpers
{
    public static class StringExtensions
    {
        public static string NormalizeNewLine(this string txt)
        {
            return txt.Replace("\r", "");
        }
    }
}