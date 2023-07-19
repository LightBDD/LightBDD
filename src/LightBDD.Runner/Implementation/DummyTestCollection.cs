using Xunit.Abstractions;
using Xunit.Sdk;

namespace LightBDD.XUnit2.Implementation;

internal class DummyTestCollection
{
    public static ITestCollection Create(ITestAssembly assembly)
    {
        return new TestCollection(assembly, new ReflectionTypeInfo(typeof(DummyTestCollection)), "LightBDD Scenarios");
    }
}