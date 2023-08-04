#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Metadata;

namespace LightBDD.Core.Execution.Implementation;

//TODO: review
internal class MetadataProvider : CoreMetadataProvider
{
    private readonly TestSuite _testSuite;

    public MetadataProvider(Assembly testAssembly, LightBddConfiguration configuration) : base(configuration)
    {
        _testSuite = TestSuite.Create(testAssembly);
    }

    protected override IEnumerable<string> GetImplementationSpecificScenarioCategories(MemberInfo member) => Enumerable.Empty<string>();

    protected override string? GetImplementationSpecificFeatureDescription(Type featureType) => null;
    protected override TestSuite GetTestSuite() => _testSuite;
}