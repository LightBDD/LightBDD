using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LightBDD.Core.Metadata.Implementation;

internal class TestRunInfo : ITestRunInfo
{
    public TestRunInfo(TestSuite testSuite, IReadOnlyList<Assembly> lightBddAssemblies)
    {
        TestSuite = testSuite;
        LightBddVersions = lightBddAssemblies.Select(assembly => new AssemblyVersion(assembly)).ToArray();
        Name = new NameInfo(testSuite.Name, Array.Empty<INameParameterInfo>());
    }


    public INameInfo Name { get; }
    public Guid RuntimeId { get; } = Guid.NewGuid();
    public TestSuite TestSuite { get; }
    public IReadOnlyList<AssemblyVersion> LightBddVersions { get; }
}