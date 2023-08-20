using LightBDD.Core.Formatting;

namespace LightBDD.Core.UnitTests.Helpers;

internal class UnderscoreToSpaceFormatter : INameFormatter
{
    public string FormatName(string name) => name.Replace('_', ' ');
}