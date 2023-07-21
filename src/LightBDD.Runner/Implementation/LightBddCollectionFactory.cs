using System.Linq;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace LightBDD.Runner.Implementation;

internal class LightBddCollectionFactory : IXunitTestCollectionFactory
{
    private readonly ITestAssembly _testAssembly;
    private readonly IXunitTestCollectionFactory _default;

    public string DisplayName => "LightBDD Collection Factory";

    public LightBddCollectionFactory(IAssemblyInfo assemblyInfo, IMessageSink diagnosticMessageSink)
    {
        var collectionBehaviourAttribute = assemblyInfo.GetCustomAttributes(typeof(CollectionBehaviorAttribute)).SingleOrDefault();

        _testAssembly = new TestAssembly(assemblyInfo);
        _default = ExtensibilityPointFactory.GetXunitTestCollectionFactory(diagnosticMessageSink, collectionBehaviourAttribute, _testAssembly);
    }

    public ITestCollection Get(ITypeInfo testClass)
    {
        if (HasLightBddCollection(testClass))
            return LightBddTestCollection.Create(_testAssembly);
        return _default.Get(testClass);
    }

    private bool HasLightBddCollection(ITypeInfo testClass)
    {
        var collectionAttribute = testClass.GetCustomAttributes(typeof(CollectionAttribute)).FirstOrDefault();
        return Equals(collectionAttribute?.GetConstructorArguments().First(), LightBddTestCollection.Name);
    }
}