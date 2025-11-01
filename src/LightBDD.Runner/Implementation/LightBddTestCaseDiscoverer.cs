using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using LightBDD.Core.Discovery;
using Xunit.v3;

namespace LightBDD.Runner.Implementation;

internal class LightBddTestCaseDiscoverer : IXunitTestCaseDiscoverer
{
    private readonly ScenarioDiscoverer _discoverer = new();

    public async ValueTask<IReadOnlyCollection<IXunitTestCase>> Discover(
        IFactAttribute factAttribute,
        IXunitTestMethod testMethod,
        ITestFrameworkDiscoveryOptions discoveryOptions)
    {
        var method = testMethod.Method;
        var type = testMethod.TestClass.Class;
        
        var scenarios = _discoverer.DiscoverFor(type, method, CancellationToken.None);
        
        var testCases = new List<IXunitTestCase>();
        
        foreach (var scenario in scenarios)
        {
            var testCase = new XunitTestCase(
                testMethod: testMethod,
                testCaseDisplayName: scenario.ScenarioName ?? testMethod.TestMethodName,
                uniqueID: null, // Will be auto-generated
                @explicit: false,
                skipExceptions: null,
                skipReason: null,
                skipType: null,
                skipUnless: null,
                skipWhen: null,
                traits: null,
                testMethodArguments: scenario.ScenarioArguments.ToNullIfEmpty(),
                sourceFilePath: null,
                sourceLineNumber: null,
                timeout: null
            );
            
            testCases.Add(testCase);
        }
        
        return await new ValueTask<IReadOnlyCollection<IXunitTestCase>>(testCases);
    }
}