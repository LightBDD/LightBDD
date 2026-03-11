using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit.Sdk;
using Xunit.v3;

namespace LightBDD.XUnit3.Implementation.Customization
{
    internal class ScenarioTestCaseDiscoverer : IXunitTestCaseDiscoverer
    {
        private readonly FactDiscoverer _factDiscoverer = new();
        private readonly TheoryDiscoverer _theoryDiscoverer = new();

        public ValueTask<IReadOnlyCollection<IXunitTestCase>> Discover(
            ITestFrameworkDiscoveryOptions discoveryOptions,
            IXunitTestMethod testMethod,
            IFactAttribute factAttribute)
        {
            return testMethod.Method.GetParameters().Length > 0
                ? _theoryDiscoverer.Discover(discoveryOptions, testMethod, factAttribute)
                : _factDiscoverer.Discover(discoveryOptions, testMethod, factAttribute);
        }
    }
}
