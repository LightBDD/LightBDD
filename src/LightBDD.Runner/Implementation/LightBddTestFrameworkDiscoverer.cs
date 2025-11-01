using System.Threading.Tasks;
using Xunit.v3;

namespace LightBDD.Runner.Implementation;

internal class LightBddTestFrameworkDiscoverer : XunitTestFrameworkDiscoverer
{
    private readonly IXunitTestCollectionFactory _collectionFactory;

    public LightBddTestFrameworkDiscoverer(IXunitTestAssembly testAssembly)
        : base(testAssembly)
    {
        _collectionFactory = new LightBddCollectionFactory(testAssembly);
    }

    protected override ValueTask<IXunitTestClass> CreateTestClass(System.Type @class)
    {
        var testCollection = _collectionFactory.Get(@class);
        return new(new XunitTestClass(@class, testCollection));
    }
}
