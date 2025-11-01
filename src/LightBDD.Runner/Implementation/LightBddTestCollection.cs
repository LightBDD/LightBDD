using Xunit.v3;

namespace LightBDD.Runner.Implementation;

internal class LightBddTestCollection
{
    public const string Name = "LightBDD Test Execution Collection";

    public static IXunitTestCollection Create(IXunitTestAssembly assembly)
    {
        return new XunitTestCollection(assembly, collectionDefinition: null, disableParallelization: false, displayName: Name);
    }
}