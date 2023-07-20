namespace LightBDD.Core.Formatting.Implementation;

//TODO: review
internal class NoNameFormatter : INameFormatter
{
    public static readonly NoNameFormatter Instance = new();
    private NoNameFormatter() { }
    public string FormatName(string name) => name;
}