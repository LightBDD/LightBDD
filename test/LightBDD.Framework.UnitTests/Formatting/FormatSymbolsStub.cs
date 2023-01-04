using LightBDD.Core.Formatting.Parameters;

namespace LightBDD.Framework.UnitTests.Formatting;

internal class FormatSymbolsStub : IFormatSymbols
{
    public string NullValue => "<null>";
    public string EmptyValue => "<empty>";
}