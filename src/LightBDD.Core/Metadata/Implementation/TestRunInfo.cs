using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LightBDD.Core.Metadata.Implementation;

internal class TestRunInfo : ITestRunInfo
{
    public TestRunInfo(TestSuite testSuite, IReadOnlyCollection<Assembly> lightBddAssemblies)
    {
        TestSuite = testSuite;
        LightBddAssemblies = lightBddAssemblies.Select(AssemblyInfo.From).ToArray();
        Name = new NameInfo(testSuite.Name, Array.Empty<INameParameterInfo>());
    }


    public INameInfo Name { get; }
    //TODO: use assembly full name
    public string RuntimeId { get; } = Guid.NewGuid().ToString();
    public TestSuite TestSuite { get; }
    public IReadOnlyList<AssemblyInfo> LightBddAssemblies { get; }
}