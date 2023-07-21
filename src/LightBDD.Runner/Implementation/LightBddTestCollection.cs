using Xunit.Abstractions;
using Xunit.Sdk;

namespace LightBDD.Runner.Implementation;

internal class LightBddTestCollection
{
    public const string Name = "LightBDD Test Execution Collection";

    public static ITestCollection Create(ITestAssembly assembly)
    {
        return new TestCollection(assembly, new ReflectionTypeInfo(typeof(LightBddTestCollection)), Name);
    }
}