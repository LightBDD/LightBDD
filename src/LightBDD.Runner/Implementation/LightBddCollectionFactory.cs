using System.Linq;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace LightBDD.Runner.Implementation;

internal class LightBddCollectionFactory : IXunitTestCollectionFactory
{
    private readonly IXunitTestCollectionFactory _default;
    private readonly ITestCollection _collection;
    public string DisplayName => "LightBDD Collection Factory";

    public LightBddCollectionFactory(IAssemblyInfo assemblyInfo, IMessageSink diagnosticMessageSink)
    {
        var collectionBehaviourAttribute = assemblyInfo.GetCustomAttributes(typeof(CollectionBehaviorAttribute)).SingleOrDefault();

        var testAssembly = new TestAssembly(assemblyInfo);
        _collection = LightBddTestCollection.Create(testAssembly);
        _default = ExtensibilityPointFactory.GetXunitTestCollectionFactory(diagnosticMessageSink, collectionBehaviourAttribute, testAssembly);
    }

    public ITestCollection Get(ITypeInfo testClass)
    {
        if (HasLightBddCollection(testClass))
            return _collection;
        return _default.Get(testClass);
    }

    private bool HasLightBddCollection(ITypeInfo testClass)
    {
        var collectionAttribute = testClass.GetCustomAttributes(typeof(CollectionAttribute)).FirstOrDefault();
        return Equals(collectionAttribute?.GetConstructorArguments().First(), LightBddTestCollection.Name);
    }
}