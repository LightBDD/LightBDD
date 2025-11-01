using System.Linq;
using Xunit;
using Xunit.v3;

namespace LightBDD.Runner.Implementation;

internal class LightBddCollectionFactory : IXunitTestCollectionFactory
{
    private readonly IXunitTestCollectionFactory _default;
    private readonly IXunitTestCollection _collection;
    public string DisplayName => "LightBDD Collection Factory";

    public LightBddCollectionFactory(IXunitTestAssembly testAssembly)
    {
        var collectionBehaviorAttribute = testAssembly.Assembly
            .GetCustomAttributes(typeof(CollectionBehaviorAttribute))
            .SingleOrDefault();

        _collection = LightBddTestCollection.Create(testAssembly);
        _default = ExtensibilityPointFactory.GetTestCollectionFactory(collectionBehaviorAttribute, testAssembly);
    }

    public IXunitTestCollection Get(System.Type testClass)
    {
        if (HasLightBddCollection(testClass))
            return _collection;
        return _default.Get(testClass);
    }

    private bool HasLightBddCollection(System.Type testClass)
    {
        var collectionAttribute = testClass.GetCustomAttributes(typeof(CollectionAttribute), true).FirstOrDefault();
        if (collectionAttribute is CollectionAttribute attr)
        {
            return attr.Name == LightBddTestCollection.Name;
        }
        return false;
    }
}