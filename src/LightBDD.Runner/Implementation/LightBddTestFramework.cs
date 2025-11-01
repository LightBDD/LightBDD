using System.Reflection;
using Xunit.v3;

namespace LightBDD.Runner.Implementation;

internal class LightBddTestFramework : XunitTestFramework
{
    public LightBddTestFramework(IXunitTestAssembly testAssembly) : base(testAssembly)
    {
    }

    protected override IXunitTestFrameworkDiscoverer CreateDiscoverer() =>
        new LightBddTestFrameworkDiscoverer(TestAssembly);

    protected override IXunitTestFrameworkExecutor CreateExecutor() =>
        new LightBddTestFrameworkExecutor(TestAssembly);
}